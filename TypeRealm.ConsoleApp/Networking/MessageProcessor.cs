using System;
using System.Threading.Tasks;
using System.Timers;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Networking
{
    // TODO: Double check for deadlocks and locking threads.

    /// <summary>
    /// This should be the only class that calls _connection.Read().
    /// </summary>
    public sealed class MessageProcessor : IMessageProcessor
    {
        private readonly object _lock = new object();
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMessageDispatcher _dispatcher;
        private readonly Authorize _authorizeMessage;
        private readonly Timer _heartbeat = new Timer(5000);
        private IConnection _connection;
        private Task _listening;
        private Task _reconnecting;

        // If connection is unsuccessful - constructor throws (from Connect method).
        public MessageProcessor(IConnectionFactory connectionFactory, IMessageDispatcher messageDispatcher, Authorize authorizeMessage)
        {
            _connectionFactory = connectionFactory;
            _dispatcher = messageDispatcher;
            _authorizeMessage = authorizeMessage;

            // When client did not hear from server - reconnect.
            _heartbeat.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                Reconnect();
            };

            Connect();
        }

        public bool IsConnected { get; private set; }

        /// <summary>
        /// Throws if not connected or sending message has failed.
        /// </summary>
        public void Send(object message)
        {
            if (_reconnecting != null)
                _reconnecting.TryWait();

            if (!IsConnected)
                throw new InvalidOperationException("Not connected.");

            // TODO: If this throws (server is down) - try reconnecting and sending again (maybe put messages to queue, buffer them).
            _connection.Write(message);
        }

        public void Dispose()
        {
            IsConnected = false; // Makes sure all tasks can exit now.

            // Dispose of connection before waiting for listener to end.
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            _reconnecting?.TryWait();
            _reconnecting = null;

            _listening?.TryWait();
            _listening = null;

            _heartbeat.Dispose();
        }

        // If connection is unsuccessful - this method throws.
        private void Connect()
        {
            // Connect and authorize.
            _connection = _connectionFactory.Connect();
            _connection.Write(_authorizeMessage);

            // Tell all processes that they can work.
            IsConnected = true;

            // Set up constant listening in background.
            _listening = Task.Run(StartListening);
            _heartbeat.Start();
        }

        private void Reconnect()
        {
            // If message processor was disposed - don't try to reconnect (don't block the thread).
            if (!IsConnected)
                return;

            _heartbeat.Stop();
            _reconnecting?.TryWait(); // In case there's another reconnection in process - wait for it to finish.
            _reconnecting = Task.Run(() =>
            {
                _connection?.Dispose();
                _listening.TryWait(); // After connection has been disposed - should throw.

                // Connect and authorize.
                _connection = _connectionFactory.Connect();
                _connection.Write(_authorizeMessage);
            });

            var success = _reconnecting.TryWait();

            // If reconnection did not succeed - stop message processor.
            if (!success)
            {
                Dispose();
                return;
            }

            _reconnecting = null; // Tell all processes that they can continue working.
            _listening = Task.Run(StartListening);
            _heartbeat.Start();
        }

        private void StartListening()
        {
            try
            {
                while (IsConnected && _reconnecting == null)
                {
                    var message = _connection.Read();

                    if (message is HeartBeat)
                    {
                        _heartbeat.Stop();
                        _heartbeat.Start();
                        continue;
                    }

                    lock (_lock)
                    {
                        _dispatcher.Dispatch(message);
                    }

                    if (message is Disconnected)
                        IsConnected = false;
                }
            }
            catch
            {
                // When we try to reconnect - we come here because we dispose of
                // connection and read operation fails. We don't want to throw.
                // We want to end this method to allow reconnection process to
                // succeed (it waits for listening process to finish).
                if (_reconnecting != null)
                    return;

                // Free this method because Reconnect() will wait for it to finish.
                // If reconnection will fail - connection will be disposed.
                Task.Run(Reconnect);
            }
        }
    }
}
