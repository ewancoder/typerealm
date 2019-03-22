using System;

namespace TypeRealm.Domain
{
    public sealed class Player
    {
        public Player(Guid playerId, Guid accountId, string name)
        {
            PlayerId = playerId;
            AccountId = accountId;
        }

        public Guid PlayerId { get; }
        public Guid AccountId { get; }
        public string Name { get; }
    }
}
