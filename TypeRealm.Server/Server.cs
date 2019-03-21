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
        private readonly object _lock = new object();
        private TcpListener _listener;

        private readonly IPlayerRepository _playerRepository;

        public Server(int port, ILogger logger, IPlayerRepository playerRepository)
        {
            _logger = logger;
            _connectedClients = new List<ConnectedClient>();
            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleConnection, _listener);

            _playerRepository = playerRepository;
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
                    HandleClient(stream);
                }
            }
            catch (Exception exception)
            {
                _logger.Log($"A client tried and failed to connect.", exception);
            }
        }

        private void HandleClient(Stream stream)
        {
            var authorizeMessage = MessageSerializer.Read(stream) as Authorize;

            var player = _playerRepository.AuthenticateOrCreate(authorizeMessage.Login, authorizeMessage.Password);

            if (player == null)
            {
                MessageSerializer.Write(stream, new Disconnected
                {
                    Reason = DisconnectReason.InvalidCredentials
                });

                _logger.Log($"Client tried to connect with invalid credentials.");
                return;
            }

            var playerId = player.PlayerId;

            var client = new ConnectedClient(playerId, stream);

            lock (_lock)
            {
                _connectedClients.Add(client);
                _logger.Log($"{playerId} has connected.");
            }

            try
            {
                while (true)
                {
                    var message = MessageSerializer.Read(stream);

                    lock (_lock)
                    {
                        _logger.Log($"Received message: {message}");

                        if (message is Quit)
                        {
                            _connectedClients.Remove(client);

                            // Used to acknowledge that client has quit.
                            MessageSerializer.Write(stream, new Disconnected());

                            _logger.Log($"{playerId} gracefully quit.");
                            return;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                lock (_lock)
                {
                    _connectedClients.Remove(client);
                    _logger.Log($"{playerId} unexpectedly lost connection.", exception);
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
