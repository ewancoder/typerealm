using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TypeRealm.Domain;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Server
{
    // TODO: Update only clients that need update.
    // TODO: Handle situation when message couldn't be sent to client.
    internal sealed class Server : IDisposable
    {
        private readonly object _lock = new object();
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IPlayerRepository _playerRepository;
        private readonly List<ConnectedClient> _connectedClients;

        private IDisposable _listener;
        private Timer _heartbeat;

        public Server(
            int port,
            ILogger logger,
            IAuthorizationService authorizationService,
            IMessageDispatcher messageDispatcher,
            IPlayerRepository playerRepository,
            IClientListenerFactory clientListenerFactory)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _messageDispatcher = messageDispatcher;
            _playerRepository = playerRepository;
            _connectedClients = new List<ConnectedClient>();

            _heartbeat = new Timer(1000);
            _heartbeat.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                lock (_lock)
                {
                    foreach (var client in _connectedClients)
                    {
                        client.Connection.TryWrite(new HeartBeat());
                    }
                }
            };
            _heartbeat.Start();

            _listener = clientListenerFactory.StartListening(port, HandleConnection);
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
            }

            if (_heartbeat != null)
            {
                _heartbeat.Dispose();
                _heartbeat = null;
            }
        }

        private void HandleConnection(IConnection connection)
        {
            var authorizeMessage = connection.Read() as Authorize;
            if (authorizeMessage == null)
                return;

            var playerName = new PlayerName(authorizeMessage.PlayerName);

            ConnectedClient client = null;
            lock (_lock)
            {
                var playerId = _authorizationService.AuthorizeOrCreate(
                    authorizeMessage.Login, authorizeMessage.Password, playerName);

                if (playerId == null)
                {
                    connection.Write(new Disconnected
                    {
                        Reason = DisconnectReason.InvalidCredentials
                    });

                    return; // Unsuccessful login.
                }

                client = new ConnectedClient(playerId, connection);

                _connectedClients.Add(client);
                _logger.Log($"{client.PlayerId} has connected.");
            }

            try
            {
                UpdateAll();

                while (true)
                {
                    var message = connection.Read();

                    lock (_lock)
                    {
                        if (message is Quit)
                        {
                            _connectedClients.Remove(client);

                            // Used to acknowledge that client has quit.
                            connection.Write(new Disconnected());
                            _logger.Log($"{client.PlayerId} gracefully quit.");
                            UpdateAll();
                            return;
                        }

                        _messageDispatcher.Dispatch(client, message);
                    }

                    UpdateAll();
                }
            }
            catch (Exception exception)
            {
                lock (_lock)
                {
                    _connectedClients.Remove(client);
                    _logger.Log($"{client.PlayerId} unexpectedly lost connection.", exception);
                }

                UpdateAll();
            }
        }

        private void UpdateAll()
        {
            lock (_lock)
            {
                foreach(var client in _connectedClients)
                {
                    if (!TrySendStatus(client))
                    {
                        _logger.Log($"Failed to send update status to {client.PlayerId} player.");
                    }
                }
            }
        }

        // The user of this method (as well as MakeStatus) should lock this operation.
        private bool TrySendStatus(ConnectedClient client)
        {
            var player = _playerRepository.Find(client.PlayerId);

            if (player == null)
                throw new InvalidOperationException($"Player {client.PlayerId} does not exist.");

            var status = MakeStatus(player);

            return client.Connection.TryWrite(status);
        }

        private Status MakeStatus(Player player)
        {
            var neighbors = _connectedClients
                .Select(c => _playerRepository.Find(c.PlayerId))
                .Where(c => c.IsAtSamePlaceAs(player) && c.PlayerId != player.PlayerId)
                .Select(c => c.Name.Value)
                .ToList();

            var status = new Status
            {
                Name = player.Name,
                LocationId = player.LocationId,
                Neighbors = neighbors
            };

            if (player.MovementInformation != null)
            {
                status.MovementStatus = new MovementStatus
                {
                    RoadId = player.MovementInformation.Road.RoadId,
                    Direction = (MovementDirection)player.MovementInformation.Direction,
                    Progress = new MovementProgress
                    {
                        Distance = player.MovementInformation.Distance,
                        Progress = player.MovementInformation.Progress
                    }
                };
            }

            return status;
        }
    }
}
