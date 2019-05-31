using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class RoadIdTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<int>>(new RoadId(10));
        }

        [Fact]
        public void ShouldThrowWhenNegativeOrZero()
        {
            Assert.Throws<ArgumentException>(() => new LocationId(-10));
            Assert.Throws<ArgumentException>(() => new LocationId(0));
        }

        [Fact]
        public void ShouldCreate()
        {
            var id = new RoadId(10);
            Assert.Equal(10, id.Value);
        }
    }
}
