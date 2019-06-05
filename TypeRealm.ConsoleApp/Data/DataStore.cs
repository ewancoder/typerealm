using System.Collections.Generic;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class DataStore : IDataStore
    {
        private readonly Dictionary<int, Road> _roads = new Dictionary<int, Road>();
        private readonly Dictionary<int, Location> _locations = new Dictionary<int, Location>();

        public DataStore(TypeRealm.Data.Data data)
        {
            foreach (var road in data.Roads)
            {
                _roads.Add(road.RoadId, new Road(
                    new RoadSide(road.Forward.Name, road.Forward.Description),
                    new RoadSide(road.Backward.Name, road.Backward.Description)));
            }

            foreach (var location in data.Locations)
            {
                _locations.Add(location.LocationId, new Location(location.Name, location.Description));
            }
        }

        public Location GetLocation(int locationId)
        {
            if (!_locations.ContainsKey(locationId))
                return null;

            return _locations[locationId];
        }

        public Road GetRoad(int roadId)
        {
            if (!_roads.ContainsKey(roadId))
                return null;

            return _roads[roadId];
        }
    }
}
