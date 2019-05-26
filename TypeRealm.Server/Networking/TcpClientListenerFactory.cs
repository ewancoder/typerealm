using System;

namespace TypeRealm.Server.Networking
{
    internal sealed class TcpClientListenerFactory : IClientListenerFactory
    {
        private readonly ILogger _logger;

        public TcpClientListenerFactory(ILogger logger)
        {
            _logger = logger;
        }

        // Connection handler will be called for every new connection in separate thread.
        public IDisposable StartListening(int port, Action<IConnection> connectionHandler)
        {
            return new TcpClientListener(port, connectionHandler, _logger);
        }
    }
}
