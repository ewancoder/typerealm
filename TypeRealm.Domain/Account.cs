namespace TypeRealm.Domain
{
    public sealed class Account
    {
        public Account(AccountId accountId, string login, string password)
        {
            AccountId = accountId;
            Login = login;
            Password = password;
        }

        public AccountId AccountId { get; }
        public string Login { get; }
        public string Password { get; }

        public Player CreatePlayer(PlayerId playerId, PlayerName name, LocationId locationId)
        {
            return new Player(playerId, AccountId, name, locationId);
        }
    }
}
