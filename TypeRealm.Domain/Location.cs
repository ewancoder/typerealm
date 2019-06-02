using System;
using System.Collections.Generic;

namespace TypeRealm.Domain
{
    public sealed class Location
    {
        public Location(IEnumerable<RoadId> roads)
        {
            if (roads == null)
                throw new ArgumentNullException(nameof(roads));

            Roads = roads;
        }

        public IEnumerable<RoadId> Roads { get; }
    }
}
