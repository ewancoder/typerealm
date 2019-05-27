using System;

namespace TypeRealm.Server.Messaging
{
    internal interface IMessageHandlerFactory
    {
        IMessageHandler Resolve(Type messageType);
    }
}
