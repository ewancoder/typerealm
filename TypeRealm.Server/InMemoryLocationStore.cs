using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class InMemoryLocationStore : ILocationStore
    {
        private readonly LocationId _startingLocationId;

        public InMemoryLocationStore(LocationId startingLocationId)
        {
            _startingLocationId = startingLocationId;
        }

        public LocationId GetStartingLocationId()
        {
            return _startingLocationId;
        }
    }

    internal sealed class InMemoryRoadStore : IRoadStore
    {
        public Road Find(RoadId roadId)
        {
            if (roadId == 1)
                return new Road(1, new RoadPoint(1, 20), new RoadPoint(1, 40));

            return null;
        }
    }
}
