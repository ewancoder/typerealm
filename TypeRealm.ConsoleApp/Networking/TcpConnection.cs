using System.IO;
using System.Net.Sockets;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class TcpConnection : IConnection
    {
        private readonly TcpClient _client;
        private readonly Stream _stream;

        public TcpConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public object Read()
        {
            return MessageSerializer.Read(_stream);
        }

        public void Write(object message)
        {
            MessageSerializer.Write(_stream, message);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _client.Dispose();
        }
    }
}
