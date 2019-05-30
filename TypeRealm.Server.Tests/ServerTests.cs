using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TypeRealm.Domain;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;
using TypeRealm.Server.Messaging;
using TypeRealm.Server.Networking;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public sealed class TestConnectionReader
    {
        private readonly Queue _messages
            = new Queue();

        private bool _throw;

        public object Read()
        {
            while (true)
            {
                Thread.Sleep(10);

                if (_throw)
                {
                    _throw = false;
                    throw new InvalidOperationException();
                }

                if (_messages.Count > 0)
                    return _messages.Dequeue();
            }
        }

        public void Push(object message)
        {
            _messages.Enqueue(message);
        }

        public void Throw()
        {
            _throw = true;
        }
    }

    public sealed class TestConnection
    {
        private readonly TestConnectionReader _reader;
        private readonly Task _listening;

        public TestConnection(Action<IConnection> connectionHandler)
        {
            _reader = new TestConnectionReader();
            Mock = new Mock<IConnection>();
            Mock
                .Setup(x => x.Read())
                .Returns(new Func<object>(() => _reader.Read()));
            Mock.Setup(x => x.Write(Capture.In(Writes)));

            _listening = Task.Run(() => connectionHandler(Mock.Object));
        }

        public Mock<IConnection> Mock { get; }
        public List<object> Writes { get; } = new List<object>();

        public void Push(object message)
        {
            _reader.Push(message);
        }

        public void Throw()
        {
            _reader.Throw();
        }

        /// <summary>
        /// Waits till Status is accepted from server.
        /// </summary>
        public void WaitTillConnected()
        {
            WaitFor<Status>();
        }

        public void WaitFor<T>()
        {
            WaitFor<T>(x => true);
        }

        public void WaitFor(object message)
        {
            WaitFor<object>(x => x == message);
        }

        public void WaitFor<T>(Func<T, bool> predicate)
        {
            // 5 seconds timeout.
            var timeout = 5000;

            while (true)
            {
                var writesCopy = Writes.ToList(); // It can be modified in the background.

                if (Writes.Any(x => x is T && predicate((T)x)))
                    return;

                Thread.Sleep(10);
                timeout -= 10;

                if (timeout <= 0)
                    throw new TimeoutException();
            }
        }

        public void Delay(Action action)
        {
            _listening.Wait(100);

            try
            {
                action();
            }
            catch // Assert failed. Try waiting more.
            {
                _listening.Wait(1000);
                action();
            }
        }

        public void Stopped()
        {
            Assert.True(_listening.Wait(1000));
            Assert.False(_listening.IsFaulted);
        }

        public void StillListens()
        {
            Assert.False(_listening.Wait(1000));
        }

        public void Throws()
        {
            Assert.ThrowsAny<Exception>(() => _listening.Wait());
        }
    }

    public class TestMessage { }

    public sealed class ServerTests
    {
        private static class Statuses
        {
            public static PlayerId Player1 { get; } = new PlayerId(new Guid("11111111-1111-1111-1111-111111111111"));
            public static PlayerId Player2 { get; } = new PlayerId(new Guid("22222222-2222-2222-2222-222222222222"));
            public static Status P1S1 { get; } = new Status();
            public static Status P1S2 { get; } = new Status();
            public static Status P2S1 { get; } = new Status();
            public static Status P2S2 { get; } = new Status();
        }

        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IMessageDispatcher> _messageDispatcherMock;
        private readonly Mock<IStatusFactory> _statusFactoryMock;
        private readonly Mock<IClientListenerFactory> _clientListenerFactoryMock;
        private readonly Mock<IDisposable> _listenerMock;

        private Server _sut;
        private TestConnection _connection1;
        private TestConnection _connection2;

        public ServerTests()
        {
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _messageDispatcherMock = new Mock<IMessageDispatcher>();
            _statusFactoryMock = new Mock<IStatusFactory>();
            _listenerMock = new Mock<IDisposable>();
            _loggerMock = new Mock<ILogger>();
            _clientListenerFactoryMock = new Mock<IClientListenerFactory>();

            _clientListenerFactoryMock
                .Setup(x => x.StartListening(10, It.IsAny<Action<IConnection>>()))
                .Returns(_listenerMock.Object)
                .Callback<int, Action<IConnection>>((port, connectionHandler) =>
                {
                    _connection1 = new TestConnection(connectionHandler);
                    _connection2 = new TestConnection(connectionHandler);
                });

            _statusFactoryMock
                .Setup(s => s.MakeStatus(It.IsAny<PlayerId>(), It.IsAny<IEnumerable<PlayerId>>()))
                .Returns(new Status());

            _sut = new Server(
                10,
                TimeSpan.FromHours(1), // Disable heartbeat for tests.
                _loggerMock.Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _statusFactoryMock.Object,
                _clientListenerFactoryMock.Object);
        }

        [Fact]
        public void ShouldDisposeOfListener()
        {
            _sut.Dispose();
            _listenerMock.Verify(x => x.Dispose());
        }

        [Fact]
        public void ShouldListenForever()
        {
            _connection1.StillListens();
        }

        [Fact]
        public void ShouldStopOnlyFailedConnection()
        {
            _connection1.Throw();
            _connection1.Throws();
            _connection2.StillListens();
        }

        [Fact]
        public void ShouldStopListeningWhenFirstMessageIsNotAuthorize()
        {
            _connection1.Push(new TestMessage());
            _connection1.Stopped();
        }

        [Fact]
        public void ShouldStopListeningWhenAuthorizeMessageIsInvalid()
        {
            _connection1.Push(new Authorize());

            // Current implementation throws an exception. It also works.
            _connection1.Throws();
        }

        [Fact]
        public void ShouldDisconnectWhenNotAuthorized()
        {
            _authorizationServiceMock
                .Setup(x => x.AuthorizeOrCreate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PlayerName>()))
                .Returns<PlayerId>(null);

            _connection1.Push(new Authorize
            {
                Login = "login",
                Password = "password",
                PlayerName = "player name"
            });

            _connection1.Stopped();

            Assert.Single(_connection1.Writes);
            AssertWritten<Disconnected>(0, _connection1, x => x.Reason == DisconnectReason.InvalidCredentials);
        }

        [Fact]
        public void ShouldSkipUpdatingWhenCannotMakeStatus()
        {
            _statusFactoryMock
                .Setup(s => s.MakeStatus(It.IsAny<PlayerId>(), It.IsAny<IEnumerable<PlayerId>>()))
                .Returns<Status>(null);

            ConnectPlayer(_connection1, Statuses.Player1);
            _connection1.Delay(() =>
            {
                Assert.Empty(_connection1.Writes);
            });
        }

        [Fact]
        public void ShouldSendStatusWhenConnected()
        {
            var status = new Status();
            ConnectPlayer(_connection1, Statuses.Player1, status);

            _connection1.Delay(() =>
            {
                Assert.Single(_connection1.Writes);
                AssertWritten<Status>(0, _connection1, status);
            });
        }

        private void ConnectTwoPlayers()
        {
            ConnectPlayer(_connection1, Statuses.Player1, Statuses.P1S1, Statuses.P1S2);
            _connection1.WaitFor(Statuses.P1S1);

            ConnectPlayer(_connection2, Statuses.Player2, Statuses.P2S1, Statuses.P2S2);
            _connection2.WaitFor(Statuses.P2S2);
            _connection1.WaitFor(Statuses.P1S2);
        }

        [Fact]
        public void ShouldSendUpdateToOtherPlayerWhenConnected()
        {
            ConnectTwoPlayers();

            Assert.Equal(2, _connection1.Writes.Count);
            AssertWritten(0, _connection1, Statuses.P1S1);
            AssertWritten(1, _connection1, Statuses.P1S2);

            Assert.Single(_connection2.Writes);
            AssertWritten(0, _connection2, Statuses.P2S2);
        }

        [Fact]
        public void ShouldDisconnectAndSendStatusOnError()
        {
            ConnectTwoPlayers();

            Assert.Equal(2, _connection1.Writes.Count);
            AssertWritten(1, _connection1, Statuses.P1S2);

            _connection2.Throw();
            _connection2.Stopped();

            Assert.Equal(3, _connection1.Writes.Count);
            AssertWritten(2, _connection1, Statuses.P1S1);
        }

        [Fact]
        public void ShouldDispatchAndSendUpdateToEveryone()
        {
            ConnectTwoPlayers();
            _connection1.Writes.Clear();
            _connection2.Writes.Clear();

            var message = new TestMessage();
            _connection1.Push(message);
            _connection1.WaitFor(Statuses.P1S2);
            _connection2.WaitFor(Statuses.P2S2);

            _messageDispatcherMock.Verify(x => x.Dispatch(It.Is<ConnectedClient>(
                c => c.Connection == _connection1.Mock.Object
                && c.PlayerId == Statuses.Player1), message));

            Assert.Single(_connection1.Writes);
            Assert.Single(_connection2.Writes);

            AssertWritten(0, _connection1, Statuses.P1S2);
            AssertWritten(0, _connection2, Statuses.P2S2);
        }

        [Fact]
        public void ShouldHandleQuitMessage()
        {
            ConnectTwoPlayers();
            _connection1.Writes.Clear();
            _connection2.Writes.Clear();

            var message = new Quit();
            _connection1.Push(message);
            _connection1.Stopped();
            _connection2.WaitFor(Statuses.P2S1);

            Assert.Single(_connection1.Writes);
            AssertWritten<Disconnected>(0, _connection1, x => x.Reason == DisconnectReason.None);

            Assert.Single(_connection2.Writes);
            AssertWritten(0, _connection2, Statuses.P2S1);
        }

        [Fact]
        public void ShouldNotFailWhenHeartbeatFails()
        {
            _sut.Dispose();
            _sut = new Server(
                10,
                TimeSpan.FromMilliseconds(100), // Disable heartbeat for tests.
                _loggerMock.Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _statusFactoryMock.Object,
                _clientListenerFactoryMock.Object);

            _connection1.Mock
                .Setup(x => x.Write(It.IsAny<Heartbeat>()))
                .Throws<InvalidOperationException>();

            ConnectPlayer(_connection1, PlayerId.New());
            Thread.Sleep(200);

            _connection1.StillListens();
        }

        [Fact]
        public void ShouldHeartbeatEverySecondToEveryone()
        {
            _sut.Dispose();
            _sut = new Server(
                10,
                TimeSpan.FromMilliseconds(100), // Disable heartbeat for tests.
                _loggerMock.Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _statusFactoryMock.Object,
                _clientListenerFactoryMock.Object);

            ConnectTwoPlayers();

            _connection1.WaitFor<Heartbeat>();
            _connection2.WaitFor<Heartbeat>();

            var count1 = _connection1.Writes.OfType<Heartbeat>().Count();
            var count2 = _connection2.Writes.OfType<Heartbeat>().Count();

            Thread.Sleep(150);

            Assert.Equal(1, _connection1.Writes.OfType<Heartbeat>().Count() - count1);
            Assert.Equal(1, _connection2.Writes.OfType<Heartbeat>().Count() - count2);

            Thread.Sleep(100);
            Assert.Equal(2, _connection1.Writes.OfType<Heartbeat>().Count() - count1);
            Assert.Equal(2, _connection2.Writes.OfType<Heartbeat>().Count() - count2);
        }

        [Fact]
        public void ShouldDisposeOfHeartbeat()
        {
            _sut.Dispose();
            _sut = new Server(
                10,
                TimeSpan.FromMilliseconds(100), // Disable heartbeat for tests.
                _loggerMock.Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _statusFactoryMock.Object,
                _clientListenerFactoryMock.Object);

            ConnectTwoPlayers();

            _connection1.WaitFor<Heartbeat>();
            _connection2.WaitFor<Heartbeat>();

            var count1 = _connection1.Writes.OfType<Heartbeat>().Count();
            var count2 = _connection2.Writes.OfType<Heartbeat>().Count();

            Thread.Sleep(150);

            Assert.Equal(1, _connection1.Writes.OfType<Heartbeat>().Count() - count1);
            Assert.Equal(1, _connection2.Writes.OfType<Heartbeat>().Count() - count2);

            _sut.Dispose();

            Thread.Sleep(100);
            Assert.Equal(1, _connection1.Writes.OfType<Heartbeat>().Count() - count1);
            Assert.Equal(1, _connection2.Writes.OfType<Heartbeat>().Count() - count2);
        }

        private void AssertWritten<T>(int index, TestConnection connection, Func<T, bool> predicate)
        {
            var message = connection.Writes[index];

            Assert.NotNull(message);
            Assert.IsType<T>(message);
            Assert.True(predicate((T)message));
        }

        private void AssertWritten<T>(int index, TestConnection connection, T message)
            => AssertWritten<T>(index, connection, x => x.Equals(message));

        private void ConnectPlayer(TestConnection connection, PlayerId playerId, Status status = null, Status statusWithNeighbor = null)
        {
            var login = "login";
            var password = "password";
            var playerName = "player name";

            _authorizationServiceMock
                .Setup(x => x.AuthorizeOrCreate(login, password, new PlayerName(playerName)))
                .Returns(playerId);

            if (status != null)
            {
                _statusFactoryMock
                    .Setup(s => s.MakeStatus(playerId, It.Is<IEnumerable<PlayerId>>(
                        x => x.Count() == 1 && x.Contains(playerId))))
                    .Returns(status);
            }

            if (statusWithNeighbor != null)
            {
                _statusFactoryMock
                    .Setup(s => s.MakeStatus(playerId, It.Is<IEnumerable<PlayerId>>(
                        x => x.Count() == 2 && x.Contains(playerId) && x.Any(y => y != playerId))))
                    .Returns(statusWithNeighbor);
            }

            connection.Push(new Authorize
            {
                Login = login,
                Password = password,
                PlayerName = playerName
            });
        }
    }
}
