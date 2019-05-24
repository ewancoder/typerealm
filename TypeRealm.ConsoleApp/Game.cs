using System;
using System.Collections.Generic;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp
{
    public sealed class Game : IDisposable
    {
        private readonly object _lock = new object();
        private readonly IPrinter _printer;
        private readonly Queue<string> _notifications = new Queue<string>();
        private IMessageProcessor _messages;
        private Status _status;

        public Game(IConnectionFactory connectionFactory, IPrinter printer, Authorize authorizeMessage)
        {
            _printer = printer;

            var dispatcher = new GameMessageDispatcher(this);
            _messages = new MessageProcessor(connectionFactory, dispatcher, authorizeMessage, Reconnect.Default());
        }

        public bool IsRunning => _messages.IsConnected;

        public void Input(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.E)
            {
                _messages.Send(new Quit());
                return;
            }

            if (key.Key == ConsoleKey.R)
            {
                _messages.Send(new EnterRoad
                {
                    RoadId = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.M)
            {
                _messages.Send(new Move
                {
                    Distance = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.T)
            {
                _messages.Send(new TurnAround());
                return;
            }
        }

        public void Update()
        {
            lock (_lock)
            {
                _printer.Print(_status, _notifications);
            }
        }

        public void Update(Status status)
        {
            _status = status;
            Update();
        }

        // Was private.
        public void Disconnect(string reason)
        {
            // Don't call _messages.Dispose(), this leads to deadlock cause messages wait for this method to finish.
            _printer.DisconnectedWithReason(reason);
        }

        public void Reconnecting()
        {
            _printer.Reconnecting();
        }

        // Was private.
        public void Notify(string message)
        {
            _notifications.Enqueue(message);

            if (_notifications.Count > 5)
                _notifications.Dequeue();

            Update();
        }

        public void Dispose()
        {
            if (_messages != null)
            {
                _messages.Dispose();
                _messages = null;
            }
        }
    }
}
