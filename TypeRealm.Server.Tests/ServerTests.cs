using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TypeRealm.Domain;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public class ServerTests
    {
        private readonly int _port = 10;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IMessageDispatcher> _messageDispatcherMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<IClientListenerFactory> _clientListenerFactoryMock;

        public ServerTests()
        {
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _messageDispatcherMock = new Mock<IMessageDispatcher>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _clientListenerFactoryMock = new Mock<IClientListenerFactory>();

            _playerRepositoryMock
                .Setup(p => p.Find(Fixture.PlayerId()))
                .Returns(Fixture.Player());
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

            AssertCompletes(task);
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
                .Returns(new Authorize
                {
                    PlayerName = "player name"
                });

            CreateServer();

            AssertCompletes(task);
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
                .Returns(new Authorize
                {
                    PlayerName = "player name"
                });

            CreateServer();

            AssertDelayed(task, () =>
            {
                connectionMock.Verify(c => c.Write(It.Is<Disconnected>(d => d.Reason == DisconnectReason.InvalidCredentials)));
            });
        }

        [Fact]
        public void ShouldConnectAndStartWaitingForMessages()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
                            };
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertRunsWithoutStopping(task);
        }

        [Fact]
        public void ShouldConnectAndSendStatus()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
                            };
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertDelayed(task, () =>
            {
                connectionMock.Verify(c => c.Write(It.Is<Status>(
                    x => x.Name == playerName
                    && x.LocationId == Fixture.LocationId()
                    && x.MovementStatus == null)));
            });
        }

        [Fact]
        public void ShouldDisconnectWhenQuitted()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
                            };
                        case 1:
                            index++;
                            return new Quit();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertCompletes(task);
        }

        [Fact]
        public void ShouldSendDisconnectedWithNoReasonWhenQuitted()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
                            };
                        case 1:
                            index++;
                            return new Quit();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertDelayed(task, () =>
            {
                connectionMock.Verify(c => c.Write(It.Is<Disconnected>(x => x.Reason == DisconnectReason.None)));
            });
        }

        [Fact]
        public void ShouldDisconnectWhenExceptionOccurs()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
                            };
                        case 1:
                            index++;
                            throw new Exception();
                        default:
                            Thread.Sleep(Timeout.Infinite);
                            return null;
                    }
                });

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertCompletes(task);
        }

        [Fact]
        public void ShouldDispatchReceivedMessages()
        {
            var connectionMock = new Mock<IConnection>();
            Task task = null;

            _clientListenerFactoryMock
                .Setup(p => p.StartListening(_port, It.IsAny<Action<IConnection>>()))
                .Callback<int, Action<IConnection>>((port, connectionHandler) => task = Task.Run(() => connectionHandler(connectionMock.Object)));

            var playerName = Fixture.PlayerName();

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
                                PlayerName = playerName.Value
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

            var playerId = Fixture.PlayerId();
            _authorizationServiceMock
                .Setup(a => a.AuthorizeOrCreate("login", "password", playerName))
                .Returns(playerId);

            CreateServer();

            AssertDelayed(task, () =>
            {
                _messageDispatcherMock.Verify(d => d.Dispatch(
                    It.Is<ConnectedClient>(x => x.PlayerId == playerId && x.Connection == connectionMock.Object),
                    It.IsAny<Say>()));

                _messageDispatcherMock.Verify(d => d.Dispatch(
                    It.Is<ConnectedClient>(x => x.PlayerId == playerId && x.Connection == connectionMock.Object),
                    It.IsAny<Authorize>()));
            });

            AssertRunsWithoutStopping(task);
        }

        private Server CreateServer()
        {
            return new Server(
                _port,
                new Mock<ILogger>().Object,
                _authorizationServiceMock.Object,
                _messageDispatcherMock.Object,
                _playerRepositoryMock.Object,
                _clientListenerFactoryMock.Object);
        }

        private void AssertDelayed(Task task, Action action)
        {
            task.Wait(100);

            try
            {
                action();
            }
            catch // Assert failed. Try waiting more.
            {
                task.Wait(1000);
                action();
            }
        }

        private void AssertCompletes(Task task)
        {
            AssertDelayed(task, () =>
            {
                Assert.True(task.IsCompleted);
            });
        }

        private void AssertRunsWithoutStopping(Task task)
        {
            task.Wait(1000);
            Assert.False(task.IsCompleted);
        }
    }
}
