namespace TypeRealm.Domain.Tests
{
    internal static class Fixture
    {
        public static PlayerName PlayerName()
        {
            return new PlayerName("player name");
        }

        public static Account Account()
        {
            return new Account(AccountId.New(), "login", "password");
        }
    }
}
