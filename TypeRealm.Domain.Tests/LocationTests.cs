using System;
using System.Linq;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class LocationTests
    {
        [Fact]
        public void ShouldThrowWhenNullRoadsArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new Location(null));
        }

        [Fact]
        public void ShouldCreateWithNoRoads()
        {
            new Location(Enumerable.Empty<RoadId>());
        }

        [Fact]
        public void ShouldCreate()
        {
            var road1 = new RoadId(10);
            var road2 = new RoadId(20);

            var sut = new Location(new[]
            {
                road1,
                road2
            });

            Assert.Equal(2, sut.Roads.Count());
            Assert.Equal(road1, sut.Roads.ToList()[0]);
            Assert.Equal(road2, sut.Roads.ToList()[1]);
        }
    }
}
