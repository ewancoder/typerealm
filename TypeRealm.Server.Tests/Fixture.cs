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
    }
}
