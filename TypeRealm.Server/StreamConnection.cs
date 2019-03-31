using System.IO;
using TypeRealm.Messages;

namespace TypeRealm.Server
{
    internal sealed class StreamConnection : IConnection
    {
        private readonly Stream _stream;

        public StreamConnection(Stream stream)
        {
            _stream = stream;
        }

        public object Read()
        {
            return MessageSerializer.Read(_stream);
        }

        public void Write(object message)
        {
            MessageSerializer.Write(_stream, message);
        }
    }
}
