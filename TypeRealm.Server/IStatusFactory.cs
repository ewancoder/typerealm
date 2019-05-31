using System.Collections.Generic;
using TypeRealm.Domain;
using TypeRealm.Messages;

namespace TypeRealm.Server
{
    public interface IStatusFactory
    {
        Status MakeStatus(PlayerId playerId, IEnumerable<PlayerId> activePlayers);
    }
}
