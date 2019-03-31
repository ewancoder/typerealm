using System;

namespace TypeRealm.Server
{
    internal sealed class ConnectedClient
    {
        public ConnectedClient(Guid playerId, IConnection connection)
        {
            PlayerId = playerId;
            Connection = connection;
        }

        public Guid PlayerId { get; }
        public IConnection Connection { get; }
    }
}
