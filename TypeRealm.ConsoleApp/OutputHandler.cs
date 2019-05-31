using System.Collections.Generic;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public sealed class OutputHandler
    {
        private readonly IPrinter _printer;
        private readonly Queue<string> _notifications = new Queue<string>();
        private Status _status;

        public OutputHandler(IPrinter printer)
        {
            _printer = printer;
        }

        public void Update()
        {
            _printer.Print(_status, _notifications);
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
    }
}
