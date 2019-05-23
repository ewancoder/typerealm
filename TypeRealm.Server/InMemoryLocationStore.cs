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
}
