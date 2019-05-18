using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly Authorize _authorizeMessage;

        public ConnectionFactory(
            IConnectionFactory connectionFactory,
            Authorize authorizeMessage)
        {
            _connectionFactory = connectionFactory;
            _authorizeMessage = authorizeMessage;
        }

        public IConnection Connect() => new Connection(_connectionFactory, _authorizeMessage);
    }
}
