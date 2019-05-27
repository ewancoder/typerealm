using Moq;
using System;
using TypeRealm.Domain;
using TypeRealm.Server.Messaging;
using TypeRealm.Server.Networking;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public sealed class MessageDispatcherTests
    {
        private readonly Mock<IMessageHandlerFactory> _handlerFactoryMock;
        private readonly MessageDispatcher _sut;

        public MessageDispatcherTests()
        {
            _handlerFactoryMock = new Mock<IMessageHandlerFactory>();

            _sut = new MessageDispatcher(
                new Mock<IMessageDispatcher>().Object,
                _handlerFactoryMock.Object);
        }

        [Fact]
        public void ShouldThrowIfHandlerCannotBeResolved()
        {
            var connection = new Mock<IConnection>().Object;
            var client = new ConnectedClient(PlayerId.New(), connection);
            var message = new object();

            Assert.Throws<InvalidOperationException>(
                () => _sut.Dispatch(client, message));
        }

        [Fact]
        public void ShouldHandleMessage()
        {
            var connection = new Mock<IConnection>().Object;
            var client = new ConnectedClient(PlayerId.New(), connection);

            var handlerMock = new Mock<IMessageHandler>();

            var message = new TestMessage();

            _handlerFactoryMock
                .Setup(x => x.Resolve(typeof(TestMessage)))
                .Returns(handlerMock.Object);

            _sut.Dispatch(client, message);

            handlerMock.Verify(x => x.Handle(client, message));
        }
    }
}
