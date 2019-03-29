using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TypeRealm.Messages;

namespace TypeRealm.Server
{
    internal sealed class Server : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<ConnectedClient> _connectedClients;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly object _lock;
        private TcpListener _listener;

        public Server(int port, ILogger logger, IAuthorizationService authorizationService, IMessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _connectedClients = new List<ConnectedClient>();
            _authorizationService = authorizationService;
            _messageDispatcher = messageDispatcher;
            _lock = new object();

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
                    HandleStream(stream);
                }
            }
            catch (Exception exception)
            {
                _logger.Log($"A client tried and failed to connect.", exception);
            }
        }

        private void HandleStream(Stream stream)
        {
            var authorizeMessage = MessageSerializer.Read(stream) as Authorize;

            ConnectedClient client = null;
            lock (_lock)
            {
                var playerId = _authorizationService.AuthorizeOrCreate(
                    authorizeMessage.Login, authorizeMessage.Password, authorizeMessage.PlayerName);

                if (playerId == null)
                {
                    MessageSerializer.Write(stream, new Disconnected
                    {
                        Reason = DisconnectReason.InvalidCredentials
                    });

                    return; // Unsuccessful login.
                }

                client = new ConnectedClient(playerId.Value, stream);

                lock (_lock)
                {
                    _connectedClients.Add(client);
                    _logger.Log($"{client.PlayerId} has connected.");
                }
            }

            try
            {
                while (true)
                {
                    var message = MessageSerializer.Read(stream);

                    lock (_lock)
                    {
                        if (message is Quit)
                        {
                            _connectedClients.Remove(client);

                            // Used to acknowledge that client has quit.
                            MessageSerializer.Write(stream, new Disconnected());

                            _logger.Log($"{client.PlayerId} gracefully quit.");
                            return;
                        }

                        _messageDispatcher.Dispatch(client, message);
                    }
                }
            }
            catch (Exception exception)
            {
                lock (_lock)
                {
                    _connectedClients.Remove(client);
                    _logger.Log($"{client.PlayerId} unexpectedly lost connection.", exception);
                }
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
