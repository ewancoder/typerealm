using System;
using TypeRealm.Domain;

namespace TypeRealm.Server.Tests
{
    internal static class Fixture
    {
        public static PlayerName PlayerName()
        {
            return new PlayerName("player name");
        }

        public static LocationId LocationId()
        {
            return new LocationId(1);
        }

        public static PlayerId PlayerId()
        {
            return new PlayerId(new Guid("11111111-1111-1111-1111-111111111111"));
        }

        public static Player Player()
        {
            return new Account(AccountId.New(), "login", "password")
                .CreatePlayer(PlayerId(), PlayerName(), LocationId());
        }
    }
}
