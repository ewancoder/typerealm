using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerIdTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<Guid>>(PlayerId.New());
        }

        [Fact]
        public void ShouldThrowWhenEmpty()
        {
            Assert.Throws<ArgumentException>(() => new PlayerId(Guid.Empty));
        }
    }
}
