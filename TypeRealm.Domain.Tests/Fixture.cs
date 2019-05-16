using System;

namespace TypeRealm.Domain.Tests
{
    internal static class Fixture
    {
        public static PlayerId PlayerId()
        {
            return new PlayerId(new Guid("11111111-1111-1111-1111-111111111111"));
        }

        public static AccountId AccountId()
        {
            return new AccountId(new Guid("11111111-1111-1111-1111-111111111111"));
        }

        public static PlayerName PlayerName()
        {
            return new PlayerName("player name");
        }

        public static Account Account()
        {
            return new Account(AccountId(), "login", "password");
        }

        public static LocationId LocationId()
        {
            return new LocationId(1);
        }
    }
}
