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

        public Road GetRoad(int roadId)
        {
            switch (roadId)
            {
                case 1:
                    return new Road(
                        new RoadSide("East gate", "Small road goes downhill to shallow grove."),
                        new RoadSide("East hill", "The village surmounts a huge mound going up to the west from the forest."));
                case 2:
                    return new Road(
                        new RoadSide("Trail to the south", "The trail leads to the castle visible from here."),
                        new RoadSide("Trail to the forest", "Shallow forest is visible from here."));
                case 3:
                    return new Road(
                        new RoadSide("North gate", "The village lies just outside the castle."),
                        new RoadSide("South gate", "The castle is built just near the village."));
                default:
                    return null;
            }
        }
    }
}
