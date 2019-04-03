namespace TypeRealm.Domain
{
    public sealed class Player
    {
        internal Player(PlayerId playerId, AccountId accountId, string name)
        {
            PlayerId = playerId;
            AccountId = accountId;
            Name = name;
        }

        public PlayerId PlayerId { get; }
        public AccountId AccountId { get; }
        public string Name { get; }
    }
}
