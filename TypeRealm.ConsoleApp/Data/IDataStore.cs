using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Data
{
    public interface IDataStore
    {
        Location GetLocation(int locationId);
        Road GetRoad(int roadId, MovementDirection direction);
        Road GetRoad(int roadId, int locationId);
    }
}
