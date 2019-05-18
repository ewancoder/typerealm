using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp
{
    public sealed class Game : IDisposable
    {
        private readonly object _lock = new object();
        private readonly IOutput _output;
        private IConnection _connection;

        private readonly Queue<string> _notifications = new Queue<string>();

        public Game(IConnectionFactory connectionFactory, IOutput output)
        {
            _output = output;
            _connection = connectionFactory.Connect();
            IsConnected = true;

            Task.Run(() => ListenToServer());
        }

        public bool IsConnected { get; private set; }

        public void Input(ConsoleKeyInfo key)
        {
            if (!IsConnected)
                return;

            if (key.Key == ConsoleKey.E)
            {
                _connection.Write(new Quit());
                return;
            }

            if (key.Key == ConsoleKey.R)
            {
                _connection.Write(new EnterRoad
                {
                    RoadId = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.M)
            {
                _connection.Write(new Move
                {
                    Distance = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.T)
            {
                _connection.Write(new TurnAround());
                return;
            }
        }

        public void Update()
        {
            lock (_lock)
            {
                _output.Clear();

                _output.WriteLine("Notifications:");
                foreach (var notification in _notifications)
                {
                    _output.WriteLine(notification);
                }
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        private void ListenToServer()
        {
            while (IsConnected)
            {
                var message = _connection.Read();

                lock (_lock)
                {
                    Dispatch(message);
                }
            }
        }

        private void Dispatch(object message)
        {
            if (message is Disconnected disconnected)
            {
                // Stop listening.
                Disconnect(disconnected.Reason.ToString());
                return;
            }

            if (message is Say say)
            {
                Say(say.Message);
                return;
            }
        }

        private void Disconnect(string reason)
        {
            IsConnected = false;

            _output.Clear();
            _output.WriteLine($"Disconnected with reason: {reason}.");
        }

        private void Say(string message)
        {
            _notifications.Enqueue(message);
            Update();
        }
    }
}
