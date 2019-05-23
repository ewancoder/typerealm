using System;

namespace TypeRealm.Server
{
    internal interface IClientListenerFactory
    {
        IDisposable StartListening(int port, Action<IConnection> connectionHandler);
    }
}
