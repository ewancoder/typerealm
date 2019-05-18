using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public sealed class Game : IDisposable
    {
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
                _connection.Write(new Quit());
        }

        public void Update()
        {
            _output.Clear();

            _output.WriteLine("Notifications:");
            foreach (var notification in _notifications)
            {
                _output.WriteLine(notification);
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
            while (true)
            {
                var message = _connection.Read();

                if (message is Disconnected disconnected)
                {
                    Disconnect(disconnected.Reason.ToString());
                    return; // Stop listening.
                }

                if (message is Say say)
                {
                    Say(say.Message);
                    continue;
                }
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
