using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Data
{
    public static class DataStoreExtensions
    {
        public static RoadSide GetRoadSide(this IDataStore dataStore, int roadId, MovementDirection direction)
        {
            var road = dataStore.GetRoad(roadId);

            return direction == MovementDirection.Forward ? road.ForwardSide : road.BackwardSide;
        }
    }
}
