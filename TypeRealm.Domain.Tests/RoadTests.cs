using TypeRealm.Domain.Tests.Common;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class RoadTests
    {
        [Fact]
        public void ShouldCreateRoad()
        {
            var roadId = new RoadId(1);
            var fromPoint = Fixture.RoadPoint();
            var toPoint = Fixture.RoadPoint();
            var road = new Road(roadId, fromPoint, toPoint);

            Assert.Equal(roadId, road.RoadId);
            Assert.Equal(fromPoint, road.FromPoint);
            Assert.Equal(toPoint, road.ToPoint);
        }

        [Fact]
        public void ShouldCreateRoadPoint()
        {
            var locationId = new LocationId(10);
            var distance = new Distance(20);
            var roadPoint = new RoadPoint(locationId, distance);

            Assert.Equal(locationId, roadPoint.LocationId);
            Assert.Equal(distance, roadPoint.DistanceToOppositeLocation);
        }
    }
}
