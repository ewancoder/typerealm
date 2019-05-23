using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class InMemoryRoadStore : IRoadStore
    {
        public Road Find(RoadId roadId)
        {
            switch (roadId)
            {
                case 1:
                    return new Road(1, new RoadPoint(1, 40), new RoadPoint(2, 100));
                case 2:
                    return new Road(2, new RoadPoint(2, 20), new RoadPoint(3, 20));
                case 3:
                    return new Road(3, new RoadPoint(3, 10), new RoadPoint(1, 15));
                default:
                    return null;
            }
        }
    }
}
