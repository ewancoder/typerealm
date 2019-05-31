namespace TypeRealm.Domain.Tests.Common
{
    public static class Roads
    {
        public static Road FromTo(LocationId from, LocationId to)
        {
            return FromTo(from, to, 1000);
        }

        public static Road FromTo(LocationId from, LocationId to, Distance distance)
        {
            return FromTo(from, distance, to, distance);
        }

        public static Road FromTo(LocationId from, Distance forwardDistance, LocationId to, Distance backwardDistance)
        {
            return new Road(
                Fixture.RoadId(),
                new RoadPoint(from, forwardDistance),
                new RoadPoint(to, backwardDistance));
        }
    }
}
