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

        // Prefix to all string values.
        public static Player Player(PlayerId playerId, PlayerName playerName, LocationId locationId)
        {
            return new Account(AccountId.New(), "login", "password")
                .CreatePlayer(playerId, playerName, locationId);
        }

        public static Player Player(PlayerId playerId, RoadId roadId, Distance distance, Distance progress)
        {
            var locationId = LocationId();

            var road = new Road(
                roadId,
                new RoadPoint(locationId, distance),
                new RoadPoint(new LocationId(25), distance));

            var movementInformation = MovementInformation
                .EnterRoadFrom(road, locationId)
                .Move(progress);

            return Domain.Player.InState(
                playerId, AccountId.New(), PlayerName(), locationId, movementInformation);
        }
    }
}
