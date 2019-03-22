using System;

namespace TypeRealm.Domain
{
    public sealed class Player
    {
        internal Player(Guid playerId, Guid accountId, string name)
        {
            PlayerId = playerId;
            AccountId = accountId;
            Name = name;
        }

        public Guid PlayerId { get; }
        public Guid AccountId { get; }
        public string Name { get; }
    }
}
