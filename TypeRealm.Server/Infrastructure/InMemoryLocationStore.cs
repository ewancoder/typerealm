using TypeRealm.Domain;

namespace TypeRealm.Server.Infrastructure
{
    /*internal sealed class InMemoryLocationStore : ILocationStore
    {
        private readonly LocationId _startingLocationId;

        public InMemoryLocationStore(LocationId startingLocationId)
        {
            _startingLocationId = startingLocationId;
        }

        public Location GetLocation(LocationId locationId)
        {
            var road1 = new RoadId(1);
            var road2 = new RoadId(2);
            var road3 = new RoadId(3);

            switch (locationId.Value)
            {
                case 1:
                    return new Location(new[] { road1, road3 });
                case 2:
                    return new Location(new[] { road1, road2 });
                case 3:
                    return new Location(new[] { road2, road3 });
                default:
                    return null;
            }
        }

        public LocationId GetStartingLocationId()
        {
            return _startingLocationId;
        }
    }*/
}
