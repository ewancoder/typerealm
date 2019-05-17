using System.Net.Sockets;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class TcpConnectionFactory : IConnectionFactory
    {
        private readonly string _server;
        private readonly int _port;

        public TcpConnectionFactory(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public INetworkConnection Connect()
        {
            var client = new TcpClient();
            client.Connect(_server, _port);

            return new TcpConnection(client);
        }
    }
}
