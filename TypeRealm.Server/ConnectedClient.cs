using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class ConnectedClient
    {
        public ConnectedClient(PlayerId playerId, IConnection connection)
        {
            PlayerId = playerId;
            Connection = connection;
        }

        public PlayerId PlayerId { get; }
        public IConnection Connection { get; }
    }
}
