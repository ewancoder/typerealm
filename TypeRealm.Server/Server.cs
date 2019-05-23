using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TypeRealm.Domain;
using TypeRealm.Messages.Connection;

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
        private readonly IStatusFactory _statusFactory;
        private readonly List<ConnectedClient> _connectedClients;

        private IDisposable _listener;
        private Timer _heartbeat;

        public Server(
            int port,
            TimeSpan heartbeatInterval,
            ILogger logger,
            IAuthorizationService authorizationService,
            IMessageDispatcher messageDispatcher,
            IStatusFactory statusFactory,
            IClientListenerFactory clientListenerFactory)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _messageDispatcher = messageDispatcher;
            _statusFactory = statusFactory;
            _connectedClients = new List<ConnectedClient>();

            _heartbeat = new Timer(heartbeatInterval.TotalMilliseconds);
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
            // TODO: Make sure this method returns and doesn't run in background
            // after Dispose was called or client was disconnected for some reason.

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

                // Update all clients without releasing lock.
                // If clients c1 and c2 has connected together in parallel and
                // released their locks - status will be send to them twice.
                UpdateAll();
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
                    try
                    {
                        var status = _statusFactory.MakeStatus(
                            client.PlayerId, _connectedClients.Select(c => c.PlayerId));

                        if (status == null)
                            throw new InvalidOperationException($"Could not make status for {client.PlayerId}.");

                        client.Connection.Write(status);
                    }
                    catch (Exception exception)
                    {
                        _logger.Log($"Failed to send update status to {client.PlayerId} player.", exception);
                    }
                }
            }
        }
    }
}
