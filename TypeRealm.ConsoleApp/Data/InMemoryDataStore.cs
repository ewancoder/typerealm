using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Data
{
    /*
     * Village (1) --(road 1)--> Forest (2)
     *     ^                        |
     *      \                    (road 2)
     *   (road 3)                   |
     *         \                    V
     *          *--------------- Castle (3)
     */
    public sealed class InMemoryDataStore : IDataStore
    {
        public Location GetLocation(int locationId)
        {
            switch (locationId)
            {
                case 1:
                    return new Location("Village", "Starting village");
                case 2:
                    return new Location("Forest", "Small forest");
                case 3:
                    return new Location("Castle", "Huge castle");
                default:
                    return null;
            }
        }

        public Road GetRoad(int roadId, MovementDirection direction)
        {
            switch (roadId)
            {
                case 1:
                    return direction == MovementDirection.Forward
                        ? new Road("East gate", "From village to forest. Small road goes downhill to shallow grove.")
                        : new Road("East hill", "From forest to village. The village surmounts a huge mound going up to the west from the forest.");
                case 2:
                    return direction == MovementDirection.Forward
                        ? new Road("Trail to the south", "From forest to castle. The trail leads to the castle visible from here.")
                        : new Road("Trail to the forest", "From castle to forest. Shallow forest is visible from here.");
                case 3:
                    return direction == MovementDirection.Forward
                        ? new Road("North gate", "From castle to village. The village lies just outside the castle.")
                        : new Road("South gate", "From village to castle. The castle is built just near the village.");
                default:
                    return null;
            }
        }
    }
}
