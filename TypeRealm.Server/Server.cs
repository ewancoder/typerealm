using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypeRealm.Domain;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Server
{
    internal sealed class Server : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<ConnectedClient> _connectedClients;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IPlayerRepository _playerRepository;
        private readonly object _lock;

        private IDisposable _listener;

        public Server(
            int port,
            ILogger logger,
            IAuthorizationService authorizationService,
            IMessageDispatcher messageDispatcher,
            IPlayerRepository playerRepository,
            IClientListenerFactory clientListenerFactory)
        {
            _logger = logger;
            _connectedClients = new List<ConnectedClient>();
            _authorizationService = authorizationService;
            _messageDispatcher = messageDispatcher;
            _playerRepository = playerRepository;
            _lock = new object();

            _listener = clientListenerFactory.StartListening(port, HandleConnection);
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
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

                lock (_lock)
                {
                    _connectedClients.Add(client);
                    _logger.Log($"{client.PlayerId} has connected.");

                    SendStatus(client);
                }
            }

            try
            {
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
                            return;
                        }

                        _messageDispatcher.Dispatch(client, message);
                    }

                    // TODO: Update only clients that need update.
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
            }
        }

        private void UpdateAll()
        {
            // TODO: Check IsCompleted property to know if every update succeeded.
            Parallel.ForEach(_connectedClients, SendStatus);
        }

        private void SendStatus(ConnectedClient client)
        {
            var player = _playerRepository.Find(client.PlayerId);

            if (player == null)
                throw new InvalidOperationException($"Player {client.PlayerId} does not exist.");

            var status = MakeStatus(player);

            client.Connection.Write(status);
        }

        private Status MakeStatus(Player player)
        {
            var status = new Status
            {
                Name = player.Name,
                LocationId = player.LocationId
            };

            if (player.MovementInformation != null)
            {
                status.MovementStatus = new MovementStatus
                {
                    RoadId = player.MovementInformation.Road.RoadId,
                    Direction = (MovementDirection)player.MovementInformation.Direction,
                    Progress = player.MovementInformation.Progress
                };
            }

            return status;
        }
    }
}
