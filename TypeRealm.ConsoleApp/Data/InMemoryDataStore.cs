﻿using TypeRealm.Messages.Movement;

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

        public Road GetRoadFrom(int roadId, int locationId)
        {
            switch (roadId)
            {
                case 1:
                    return locationId == 1
                        ? new Road("East gate", "Small road goes downhill to shallow grove.")
                        : new Road("East hill", "The village surmounts a huge mound going up to the west from the forest.");
                case 2:
                    return locationId == 2
                        ? new Road("Trail to the south", "The trail leads to the castle visible from here.")
                        : new Road("Trail to the forest", "Shallow forest is visible from here.");
                case 3:
                    return locationId == 3
                        ? new Road("North gate", "The village lies just outside the castle.")
                        : new Road("South gate", "The castle is built just near the village.");
                default:
                    return null;
            }
        }
    }
}
