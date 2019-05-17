using System;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public sealed class Connection : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly Authorize _authorizeMessage;

        public Connection(IConnectionFactory connectionFactory, Authorize authorizeMessage)
        {
            _connectionFactory = connectionFactory;
            _authorizeMessage = authorizeMessage;
        }

        public INetworkConnection NetworkConnection { get; private set; }

        public object ReceiveMessage()
        {
            // TODO: Add pinging and reconnecting if connection is lost.
            return NetworkConnection.Read();
        }

        public void Send(object message)
        {
            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    // TODO: Add idempotency key.
                    NetworkConnection.Write(message);
                    return;
                }
                catch
                {
                    if (i == 5)
                        throw;

                    ReconnectAndAuthorize();
                }
            }
        }

        public void ReconnectAndAuthorize()
        {
            Dispose();

            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    NetworkConnection = _connectionFactory.Connect();

                    Send(_authorizeMessage);

                    break;
                }
                catch
                {
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            NetworkConnection?.Dispose();
        }
    }
}
