using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TypeRealm.Domain;
using TypeRealm.Messages;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public class ServerTests
    {
        private readonly int _port = 10;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IMessageDispatcher> _messageDispatcherMock;
        private readonly Mock<IClientListenerFactory> _clientListenerFactoryMock;

        public ServerTests()
        {
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _messageDispatcherMock = new Mock<IMessageDispatcher>();
            _clientListenerFactoryMock = new Mock<IClientListenerFactory>();
        }

        [Fact]
        public void Should_DisposeListener()
        {
            var listenerMock = new Mock<IDisposable>();

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Returns(listenerMock.Object);

            var sut = CreateServer();
            sut.Dispose();

            listenerMock.Verify(x => x.Dispose());
        }

        [Fact]
        public void ShouldReturnWhenFirstMessageIsNotAuthorize()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            CreateServer();

            task.Wait(100);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ShouldReturnWhenNotAuthorized()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            connectionMock
                .Setup(c => c.Read())
                .Returns(new Authorize());

            CreateServer();

            task.Wait(100);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ShouldSendDisconnectedWithInvalidCredentialsWhenNotAuthorized()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            connectionMock
                .Setup(c => c.Read())
                .Returns(new Authorize());

            CreateServer();

            task.Wait(100);
            connectionMock.Verify(c => c.Write(It.Is<Disconnected>(d => d.Reason == DisconnectReason.InvalidCredentials)));
        }

        [Fact]
        public void ShouldConnectAndStartWaitingForMessages()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var index = 0;
            connectionMock
                .Setup(c => c.Read())
                .Returns(() =>
                {
                    switch (index)
                    {
                        case 0:
                            index++;
                            return new Authorize
                            {
                                Login = "login",
                                Password = "password",
                                PlayerName = "playerName"
                            };
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = PlayerId.New();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", "playerName"))
                .Returns(playerId);

            CreateServer();

            task.Wait(100);
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ShouldDisconnectWhenQuitted()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var index = 0;
            connectionMock
                .Setup(c => c.Read())
                .Returns(() =>
                {
                    switch (index)
                    {
                        case 0:
                            index++;
                            return new Authorize
                            {
                                Login = "login",
                                Password = "password",
                                PlayerName = "playerName"
                            };
                        case 1:
                            index++;
                            return new Quit();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = PlayerId.New();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", "playerName"))
                .Returns(playerId);

            CreateServer();

            task.Wait(100);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ShouldSendDisconnectedWithNoReasonWhenQuitted()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var index = 0;
            connectionMock
                .Setup(c => c.Read())
                .Returns(() =>
                {
                    switch (index)
                    {
                        case 0:
                            index++;
                            return new Authorize
                            {
                                Login = "login",
                                Password = "password",
                                PlayerName = "playerName"
                            };
                        case 1:
                            index++;
                            return new Quit();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = PlayerId.New();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", "playerName"))
                .Returns(playerId);

            CreateServer();

            task.Wait(100);
            connectionMock.Verify(c => c.Write(It.Is<Disconnected>(x => x.Reason == DisconnectReason.None)));
        }

        [Fact]
        public void ShouldDisconnectWhenExceptionOccurs()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var index = 0;
            connectionMock
                .Setup(c => c.Read())
                .Returns(() =>
                {
                    switch (index)
                    {
                        case 0:
                            index++;
                            return new Authorize
                            {
                                Login = "login",
                                Password = "password",
                                PlayerName = "playerName"
                            };
                        case 1:
                            index++;
                            throw new Exception();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = PlayerId.New();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", "playerName"))
                .Returns(playerId);

            CreateServer();

            task.Wait(100);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ShouldDispatchReceivedMessages()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var index = 0;
            connectionMock
                .Setup(c => c.Read())
                .Returns(() =>
                {
                    switch (index)
                    {
                        case 0:
                            index++;
                            return new Authorize
                            {
                                Login = "login",
                                Password = "password",
                                PlayerName = "playerName"
                            };
                        case 1:
                            index++;
                            return new Say();
                        case 2:
                            index++;
                            return new Authorize();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = PlayerId.New();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", "playerName"))
                .Returns(playerId);

            CreateServer();

            task.Wait(100);
            Assert.False(task.IsCompleted);

            _messageDispatcherMock.Verify(d => d.Dispatch(
                It.Is<ConnectedClient>(x => x.PlayerId == playerId && x.Connection == connectionMock.Object),
                It.IsAny<Say>()));

            _messageDispatcherMock.Verify(d => d.Dispatch(
                It.Is<ConnectedClient>(x => x.PlayerId == playerId && x.Connection == connectionMock.Object),
                It.IsAny<Authorize>()));
        }

        private Server CreateServer()
        {
            return new Server(
                _port,
                new Mock<ILogger>().Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _clientListenerFactoryMock.Object);
        }
    }
}
