namespace TypeRealm.Domain
{
    public sealed class Player
    {
        public Player(string playerId)
        {
            PlayerId = playerId;
        }

        public string PlayerId { get; }
    }
}
