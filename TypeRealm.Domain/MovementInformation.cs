using System;

namespace TypeRealm.Domain
{
    public sealed class MovementInformation
    {
        private MovementInformation(Road road, Distance progress, RoadDirection direction)
        {
            Road = road;
            Progress = progress;
            Direction = direction;
        }

        public Road Road { get; }
        public Distance Progress { get; }
        public RoadDirection Direction { get; }

        public LocationId ArrivalLocationId => Road.GetArrivalLocationId(Direction);
        public Distance Distance => Road.GetDistanceFor(Direction);
        public bool HasArrived => Progress == Distance;

        public MovementInformation Move(Distance progress)
        {
            var newProgress = Progress.Add(progress);
            var distance = Road.GetDistanceFor(Direction);

            if (newProgress.IsGreaterThan(distance))
                throw new InvalidOperationException("Can't progress beyond distance.");

            return new MovementInformation(
                Road, newProgress, Direction);
        }

        public MovementInformation TurnAround()
        {
            var newDirection = Direction.Flip();

            if (Progress.IsZero())
            {
                var newDistance = Road.GetDistanceFor(newDirection);
                return new MovementInformation(Road, newDistance, newDirection);
            }

            // Math.
            {
                // Don't remove 100d part. It specifies decimal type here.
                var oldDistanceValue = Road.GetDistanceFor(Direction).Value;
                var oldProgressValue = Progress.Value;
                var oldProgressPercentage = oldProgressValue * 100d / oldDistanceValue;
                var newProgressPercentage = 100 - oldProgressPercentage;

                var newDistanceValue = Road.GetDistanceFor(newDirection).Value;
                var newProgressValue = newDistanceValue * newProgressPercentage / 100d;
                var newProgress = new Distance(Round(newProgressValue));

                return new MovementInformation(Road, newProgress, newDirection);
            }
        }

        public static MovementInformation EnterRoadFrom(Road road, LocationId locationId)
        {
            if (road.FromPoint.LocationId == locationId)
                return new MovementInformation(road, Distance.Zero, RoadDirection.Forward);

            if (road.ToPoint.LocationId == locationId)
                return new MovementInformation(road, Distance.Zero, RoadDirection.Backward);

            throw new InvalidOperationException($"Can't enter road {road.RoadId} from location {locationId}.");
        }

        // TODO: Move to common place (Common assembly for instance).
        private static int Round(double value)
        {
            return (int)Math.Floor(value);
        }
    }
}
