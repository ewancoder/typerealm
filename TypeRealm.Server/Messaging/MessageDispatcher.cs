using System;

namespace TypeRealm.Server.Messaging
{
    internal sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageDispatcher _echo;
        private readonly IMessageHandlerFactory _handlerFactory;

        public MessageDispatcher(
            IMessageDispatcher echo,
            IMessageHandlerFactory handlerFactory)
        {
            _echo = echo;
            _handlerFactory = handlerFactory;
        }

        public void Dispatch(ConnectedClient client, object message)
        {
            _echo.Dispatch(client, message);
            var type = message.GetType();

            var handler = _handlerFactory.Resolve(type);
            if (handler == null)
                throw new InvalidOperationException("Message handler is not registered.");

            handler.Handle(client, message);
        }
    }
}
