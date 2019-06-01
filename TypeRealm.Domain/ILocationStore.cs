namespace TypeRealm.Domain
{
    public interface ILocationStore
    {
        LocationId GetStartingLocationId();
        Location GetLocation(LocationId locationId);
    }
}
