using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class LocationIdTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<int>>(new LocationId(10));
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
            var id = new LocationId(10);
            Assert.Equal(10, id.Value);
        }
    }
}
