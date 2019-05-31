using System;

namespace TypeRealm.Server.Networking
{
    internal interface IClientListenerFactory
    {
        IDisposable StartListening(int port, Action<IConnection> connectionHandler);
    }
}
