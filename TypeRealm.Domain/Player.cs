namespace TypeRealm.Domain
{
    public sealed class Player
    {
        internal Player(PlayerId playerId, AccountId accountId, PlayerName name)
        {
            PlayerId = playerId;
            AccountId = accountId;
            Name = name;
        }

        public PlayerId PlayerId { get; }
        public AccountId AccountId { get; }
        public PlayerName Name { get; }
    }
}
