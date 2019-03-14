using System.IO;

namespace TypeRealm.Server
{
    internal sealed class ConnectedClient
    {
        public ConnectedClient(string playerId, Stream stream)
        {
            PlayerId = playerId;
            Stream = stream;
        }

        public string PlayerId { get; }
        public Stream Stream { get; }
    }
}
