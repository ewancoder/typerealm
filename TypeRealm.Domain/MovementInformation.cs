using System;

namespace TypeRealm.Domain
{
    public sealed class MovementInformation
    {
        private readonly Road _road;
        private readonly Distance _progress;
        private readonly RoadDirection _direction;

        private MovementInformation(Road road, Distance progress, RoadDirection direction)
        {
            _road = road;
            _progress = progress;
            _direction = direction;
        }

        public RoadId RoadId => _road.RoadId;
        public LocationId ArrivalLocationId => _road.GetArrivalLocationId(_direction);
        public bool HasArrived => _progress == _road.GetDistanceFor(_direction);

        public MovementInformation Move(Distance progress)
        {
            var newProgress = _progress.Add(progress);
            var distance = _road.GetDistanceFor(_direction);

            if (newProgress.IsGreaterThan(distance))
                throw new InvalidOperationException("Can't progress beyond distance.");

            return new MovementInformation(
                _road, _progress, _direction);
        }

        public MovementInformation TurnAround()
        {
            var newDirection = _direction.Flip();

            if (_progress.IsZero())
            {
                var newDistance = _road.GetDistanceFor(newDirection);
                return new MovementInformation(_road, newDistance, newDirection);
            }

            // Math.
            {
                // Don't remove 100d part. It specifies decimal type here.
                var oldDistanceValue = _road.GetDistanceFor(_direction).Value;
                var oldProgressValue = _progress.Value;
                var oldProgressPercentage = oldProgressValue * 100d / oldDistanceValue;

                var newDistanceValue = _road.GetDistanceFor(newDirection).Value;
                var newProgressValue = newDistanceValue * oldProgressPercentage / 100d;
                var newProgress = new Distance(Round(newProgressValue));

                return new MovementInformation(_road, newProgress, newDirection);
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
