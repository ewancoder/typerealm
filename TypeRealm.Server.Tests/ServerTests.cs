using System;
using System.IO;
using Moq;
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
                .Setup(p => p.StartListening(_port, It.IsAny<Action<Stream>>()))
                .Returns(listenerMock.Object);

            var sut = CreateServer();
            sut.Dispose();

            listenerMock.Verify(x => x.Dispose());
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
