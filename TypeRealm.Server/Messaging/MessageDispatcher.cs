using System;
using System.Collections.Generic;

namespace TypeRealm.Server.Messaging
{
    internal sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageDispatcher _echo;
        private readonly Dictionary<Type, IMessageHandler> _handlers;

        public MessageDispatcher(
            IMessageDispatcher echo,
            Dictionary<Type, IMessageHandler> handlers)
        {
            _echo = echo;
            _handlers = handlers; // TODO: Make immutable / encapsulate to handler-factory.
        }

        public void Dispatch(ConnectedClient client, object message)
        {
            _echo.Dispatch(client, message);
            var type = message.GetType();

            if (!_handlers.ContainsKey(type))
                throw new InvalidOperationException("Message handler is not registered.");

            var handler = _handlers[type];

            handler.Handle(client, message);
        }
    }
}
