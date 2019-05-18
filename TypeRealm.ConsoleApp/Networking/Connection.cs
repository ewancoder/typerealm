using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class Connection : IConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly Authorize _authorizeMessage;
        private IConnection _networkConnection;

        public Connection(IConnectionFactory connectionFactory, Authorize authorizeMessage)
        {
            _connectionFactory = connectionFactory;
            _authorizeMessage = authorizeMessage;
            ReconnectAndAuthorize();

            if (_networkConnection == null)
                throw new ConnectionFailedException();
        }

        public object Read()
        {
            // TODO: Add pinging and reconnecting if connection is lost.
            return _networkConnection.Read();
        }

        public void Write(object message)
        {
            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    // TODO: Add idempotency key.
                    _networkConnection.Write(message);
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

        public void Dispose()
        {
            if (_networkConnection != null)
            {
                _networkConnection.Dispose();
                _networkConnection = null;
            }
        }

        private void ReconnectAndAuthorize()
        {
            Dispose();

            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    _networkConnection = _connectionFactory.Connect();

                    Write(_authorizeMessage);

                    break;
                }
                catch
                {
                    Dispose();
                }
            }
        }
    }
}
