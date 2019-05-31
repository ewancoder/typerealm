using System;
using TypeRealm.Domain.Tests.Common;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class RoadExtensionsTests
    {
        [Fact]
        public void ShouldGetStartingPointForKnownDirections()
        {
            var fromPoint = new RoadPoint(Fixture.LocationId(), 10);
            var toPoint = new RoadPoint(Fixture.LocationId(), 20);

            var road = new Road(Fixture.RoadId(), fromPoint, toPoint);

            Assert.Equal(fromPoint, road.GetStartingPointFor(RoadDirection.Forward));
            Assert.Equal(toPoint, road.GetStartingPointFor(RoadDirection.Backward));
            Assert.Throws<ArgumentException>(() => road.GetStartingPointFor(0));
        }

        [Fact]
        public void ShouldGetArrivalPointForKnownDirections()
        {
            var fromPoint = new RoadPoint(Fixture.LocationId(), 10);
            var toPoint = new RoadPoint(Fixture.LocationId(), 20);

            var road = new Road(Fixture.RoadId(), fromPoint, toPoint);

            Assert.Equal(fromPoint, road.GetArrivalPointFor(RoadDirection.Backward));
            Assert.Equal(toPoint, road.GetArrivalPointFor(RoadDirection.Forward));
            Assert.Throws<ArgumentException>(() => road.GetArrivalPointFor(0));
        }

        [Fact]
        public void ShouldFlipDirection()
        {
            var direction = RoadDirection.Forward;

            Assert.Equal(RoadDirection.Backward, direction.Flip());
            Assert.Equal(RoadDirection.Forward, direction.Flip().Flip());
            Assert.Throws<ArgumentException>(() => ((RoadDirection)0).Flip());
        }

        [Fact]
        public void ShouldGetDistanceForKnownDirections()
        {
            var forwardDistance = new Distance(10);
            var backwardDistance = new Distance(20);
            var fromPoint = new RoadPoint(Fixture.LocationId(), forwardDistance);
            var toPoint = new RoadPoint(Fixture.LocationId(), backwardDistance);

            var road = new Road(Fixture.RoadId(), fromPoint, toPoint);

            Assert.Equal(forwardDistance, road.GetDistanceFor(RoadDirection.Forward));
            Assert.Equal(backwardDistance, road.GetDistanceFor(RoadDirection.Backward));
        }

        [Fact]
        public void ShouldGetArrivalLocation()
        {
            var startLocation = new LocationId(10);
            var endLocation = new LocationId(20);

            var road = new Road(
                Fixture.RoadId(),
                new RoadPoint(startLocation, 100),
                new RoadPoint(endLocation, 100));

            Assert.Equal(endLocation, road.GetArrivalLocationId(RoadDirection.Forward));
            Assert.Equal(startLocation, road.GetArrivalLocationId(RoadDirection.Backward));
        }
    }
}
