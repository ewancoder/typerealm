using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class DistanceTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<int>>(new Distance(10));
        }

        [Fact]
        public void ShouldAllowZeroDistance()
        {
            var distance = new Distance(0);

            Assert.Equal(0, distance.Value);
            Assert.True(distance.IsZero);
        }

        [Fact]
        public void ShouldThrowWhenNegative()
        {
            Assert.Throws<ArgumentException>(() => new Distance(-1));
            new Distance(0); // Should not throw.
        }

        [Fact]
        public void ShouldBeImplicitlyConvertible()
        {
            int value;
            var distance = new Distance(10);
            value = distance;

            Assert.Equal(10, value);

            Distance fromValue = value;
            Assert.Equal(10, fromValue.Value);
        }

        [Fact]
        public void ShouldCreateZero()
        {
            var distance = Distance.Zero;

            Assert.Equal(0, distance.Value);
            Assert.True(distance.IsZero);
        }

        [Fact]
        public void ShouldCreatePositive()
        {
            var distance = new Distance(100);

            Assert.Equal(100, distance.Value);
            Assert.False(distance.IsZero);
        }
    }
}
