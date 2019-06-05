using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TypeRealm.Domain;

namespace TypeRealm.Data
{
    // TODO: Split this class to client and server version. No need to cache it all.
    public sealed class Database : IRoadStore, ILocationStore
    {
        private readonly LocationId _startingLocationId;
        private readonly Dictionary<LocationId, Location> _locations = new Dictionary<LocationId, Location>();
        private readonly Dictionary<RoadId, Road> _roads = new Dictionary<RoadId, Road>();

        public Database(string fileName)
        {
            // Load all the data into memory.

            var data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(fileName));

            var locationRoads = new Dictionary<int, List<RoadId>>();

            foreach (var road in data.Roads)
            {
                // Fill domain roads.
                var roadId = new RoadId(road.RoadId);
                _roads.Add(roadId, new Road(
                    roadId,
                    new RoadPoint(new LocationId(road.Forward.LocationId), road.Forward.Distance),
                    new RoadPoint(new LocationId(road.Backward.LocationId), road.Backward.Distance)));

                // Prepare to fill domain locations.
                if (!locationRoads.ContainsKey(road.Forward.LocationId))
                    locationRoads.Add(road.Forward.LocationId, new List<RoadId>());

                if (!locationRoads.ContainsKey(road.Backward.LocationId))
                    locationRoads.Add(road.Backward.LocationId, new List<RoadId>());

                locationRoads[road.Forward.LocationId].Add(new RoadId(road.RoadId));
                locationRoads[road.Backward.LocationId].Add(new RoadId(road.RoadId));
            }

            foreach (var location in data.Locations)
            {
                // First location is the starting one.
                if (_startingLocationId == null)
                    _startingLocationId = new LocationId(location.LocationId);

                // Fill server locations.
                _locations.Add(
                    new LocationId(location.LocationId),
                    new Domain.Location(locationRoads[location.LocationId]));
            }

            // TODO: Validate data, check that all paths are traversable etc.
        }

        public Road Find(RoadId roadId)
        {
            return _roads[roadId];
        }

        public Location GetLocation(LocationId locationId)
        {
            return _locations[locationId];
        }

        public LocationId GetStartingLocationId()
        {
            return _startingLocationId;
        }
    }
}
