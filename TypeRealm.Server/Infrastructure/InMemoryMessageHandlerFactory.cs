using System;
using System.Collections.Generic;
using TypeRealm.Server.Messaging;

namespace TypeRealm.Server.Infrastructure
{
    internal sealed class InMemoryMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers;

        public InMemoryMessageHandlerFactory(Dictionary<Type, IMessageHandler> handlers)
        {
            _handlers = handlers;
        }

        public IMessageHandler Resolve(Type messageType)
        {
            if (!_handlers.ContainsKey(messageType))
                return null;

            return _handlers[messageType];
        }
    }
}
