using System;
using System.Threading.Tasks;
using System.Timers;
using TypeRealm.Messages.Connection;

namespace TypeRealm.ConsoleApp.Networking
{
    /// <summary>
    /// This should be the only class that calls _connection.Read().
    /// </summary>
    public sealed class MessageProcessor : IMessageProcessor
    {
        private readonly object _lock = new object();
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMessageDispatcher _dispatcher;
        private readonly Authorize _authorizeMessage;
        private readonly Timer _heartbeat;
        private readonly Reconnect _reconnect;
        private IConnection _connection;
        private Task _listening;
        private Task _reconnecting;
        private bool _isReconnecting;

        // If connection is unsuccessful - constructor throws (from Connect method).
        public MessageProcessor(IConnectionFactory connectionFactory, IMessageDispatcher messageDispatcher, Authorize authorizeMessage, Reconnect reconnect)
        {
            _connectionFactory = connectionFactory;
            _dispatcher = messageDispatcher;
            _authorizeMessage = authorizeMessage;
            _reconnect = reconnect;

            _heartbeat = new Timer(reconnect.HeartbeatReconnectTimeout.TotalMilliseconds);

            // When client did not hear from server - reconnect.
            _heartbeat.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                Reconnect();
            };

            Connect();
        }

        public bool IsConnected { get; private set; }

        /// <summary>
        /// Tries to reconnect if sending message has failed.
        /// Doesn't do anything if trying to reconnect right now.
        /// </summary>
        public void Send(object message)
        {
            lock (_lock)
            {
                if (_isReconnecting)
                    return;

                if (!IsConnected)
                    throw new InvalidOperationException("Not connected.");
            }

            // TODO: Maybe put messages to queue, buffer them.
            // TODO: If we send multiple commands, they are "buffered" by .NET locking and then after reconnection are sent to server which leads to invalid state.
            try
            {
                _connection.Write(message);
            }
            catch
            {
                Reconnect();
            }
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

        // If connection is unsuccessful - IsConnected will remain false.
        private void Connect()
        {
            object message = null;

            try
            {
                // Connect and authorize.
                _connection = _connectionFactory.Connect();
                _connection.Write(_authorizeMessage);

                // Wait for connection acknowledgement. Server should send status.
                message = _connection.Read();
            }
            catch
            {
                // We get there if server threw an error.
                lock (_lock)
                {
                    _dispatcher.Dispatch(new Disconnected
                    {
                        Reason = DisconnectReason.CouldNotConnect
                    });
                }

                return;
            }

            lock (_lock)
            {
                _dispatcher.Dispatch(message);
            }

            if (message is Disconnected)
                return;

            // Tell all processes that they can work.
            IsConnected = true;

            // Set up constant listening in background.
            _listening = Task.Run(StartListening);
            _heartbeat.Start();
        }

        private void Reconnect()
        {
            lock (_lock)
            {
                // If message processor was disposed - don't try to reconnect (don't block the thread).
                if (!IsConnected)
                    return;

                if (_isReconnecting)
                    return;

                _isReconnecting = true;
            }

            _heartbeat.Stop();
            _reconnecting?.TryWait(); // In case there's another reconnection in process - wait for it to finish.

            for (var i = 1; i <= _reconnect.Tries; i++)
            {
                _reconnecting = Task.Run(() =>
                {
                    _connection?.Dispose();
                    _listening.TryWait(); // After connection has been disposed - should throw.

                    // Connect and authorize.
                    _connection = _connectionFactory.Connect();
                    _connection.Write(_authorizeMessage);
                });

                var success = _reconnecting.TryWait();

                if (!success)
                {
                    System.Threading.Thread.Sleep(_reconnect.RetryWaitTime);
                    continue;
                }

                // Tell all processes that they can continue working.
                lock (_lock)
                {
                    _reconnecting = null;
                    _isReconnecting = false;
                }

                _listening = Task.Run(StartListening);
                _heartbeat.Start();

                return;
            }

            // If reconnection did not succeed - stop message processor.
            Dispose();

            lock (_lock)
            {
                _dispatcher.Dispatch(new Disconnected
                {
                    Reason = DisconnectReason.LostConnection
                });
            }
        }

        private void StartListening()
        {
            try
            {
                while (IsConnected && !_isReconnecting)
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
                lock (_lock)
                {
                    if (_isReconnecting)
                        return;
                }

                // Free this method because Reconnect() will wait for it to finish.
                // If reconnection will fail - connection will be disposed.
                Task.Run(Reconnect);
            }
        }
    }
}
