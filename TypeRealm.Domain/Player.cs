using System;

namespace TypeRealm.Domain
{
    public sealed class Player
    {
        internal Player(
            PlayerId playerId,
            AccountId accountId,
            PlayerName name,
            LocationId locationId)
        {
            PlayerId = playerId;
            AccountId = accountId;
            Name = name;
            LocationId = locationId;
        }

        public PlayerId PlayerId { get; }
        public AccountId AccountId { get; }
        public PlayerName Name { get; }

        public LocationId LocationId { get; private set; }
        public MovementInformation MovementInformation { get; private set; }

        public static Player InState(
            PlayerId playerId,
            AccountId accountId,
            PlayerName name,
            LocationId locationId,
            MovementInformation movementInformation)
        {
            var player = new Player(playerId, accountId, name, locationId);
            player.MovementInformation = movementInformation;

            return player;
        }

        public bool IsAtSamePlaceAs(Player player)
        {
            return LocationId == player.LocationId
                && MovementInformation?.Road.RoadId == player.MovementInformation?.Road.RoadId;
        }

        public void EnterRoad(Road road)
        {
            if (MovementInformation != null)
                throw new InvalidOperationException($"Player {PlayerId} is already moving at {MovementInformation.Road.RoadId} road.");

            MovementInformation = MovementInformation.EnterRoadFrom(road, LocationId);
        }

        public void Move(Distance distance)
        {
            if (MovementInformation == null)
                throw new InvalidOperationException($"Player {PlayerId} is not moving.");

            MovementInformation = MovementInformation.Move(distance);

            if (MovementInformation.HasArrived)
            {
                LocationId = MovementInformation.ArrivalLocationId;
                MovementInformation = null;
            }
        }

        public void TurnAround()
        {
            if (MovementInformation == null)
                throw new InvalidOperationException($"Player {PlayerId} is not moving.");

            MovementInformation = MovementInformation.TurnAround();

            if (MovementInformation.HasArrived)
            {
                LocationId = MovementInformation.ArrivalLocationId;
                MovementInformation = null;
            }
        }
    }
}
