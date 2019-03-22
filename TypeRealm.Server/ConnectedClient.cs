using System;
using System.IO;

namespace TypeRealm.Server
{
    internal sealed class ConnectedClient
    {
        public ConnectedClient(Guid playerId, Stream stream)
        {
            PlayerId = playerId;
            Stream = stream;
        }

        public Guid PlayerId { get; }
        public Stream Stream { get; }
    }
}
