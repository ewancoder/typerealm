using System;

namespace TypeRealm.Domain.Tests.Common
{
    internal static class Fixture
    {
        public static PlayerId PlayerId()
        {
            return new PlayerId(new Guid("11111111-1111-1111-1111-111111111111"));
        }

        public static AccountId AccountId()
        {
            return new AccountId(new Guid("21111111-1111-1111-1111-111111111111"));
        }

        internal static RoadPoint RoadPoint()
        {
            return new RoadPoint(LocationId(), 10);
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

        public static RoadId RoadId()
        {
            return new RoadId(1);
        }

        public static Player PlayerAt(LocationId locationId)
        {
            return Player.InState(
                PlayerId(), AccountId(), PlayerName(), locationId, null);
        }

        public static Player PlayerAt(Road road, Distance progress, RoadDirection direction)
        {
            return Player.InState(
                PlayerId(), AccountId(), PlayerName(), LocationId(), MovementInformationAt(road, progress, direction));
        }

        public static MovementInformation MovementInformationAt(Road road, Distance progress, RoadDirection direction)
        {
            return MovementInformation
                .EnterRoadFrom(road, direction == RoadDirection.Forward ? road.FromPoint.LocationId : road.ToPoint.LocationId)
                .Move(progress);
        }
    }
}
