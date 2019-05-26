using System;
using System.Net;
using System.Net.Sockets;

namespace TypeRealm.Server.Networking
{
    internal sealed class TcpClientListener : IDisposable
    {
        private readonly Action<IConnection> _connectionHandler;
        private readonly ILogger _logger;
        private TcpListener _listener;

        public TcpClientListener(int port, Action<IConnection> connectionHandler, ILogger logger)
        {
            _connectionHandler = connectionHandler;
            _logger = logger;

            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleConnection, _listener);
        }

        private void HandleConnection(IAsyncResult result)
        {
            // Start waiting for another client as soon as any client has connected.
            _listener.BeginAcceptTcpClient(HandleConnection, _listener);

            try
            {
                using (var tcpClient = _listener.EndAcceptTcpClient(result))
                using (var stream = tcpClient.GetStream())
                {
                    // TODO: Consider making StreamConnection disposable.
                    var connection = new StreamConnection(stream);

                    _connectionHandler(connection);
                }
            }
            catch (Exception exception)
            {
                _logger.Log($"A client tried and failed to connect.", exception);
            }
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.Stop();
                _listener = null;
            }
        }
    }
}
