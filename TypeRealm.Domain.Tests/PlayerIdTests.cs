using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class PlayerIdTests
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

        [Fact]
        public void ShouldCreateUnique()
        {
            var id1 = PlayerId.New();
            var id2 = PlayerId.New();

            Assert.NotEqual(Guid.Empty, id1.Value);
            Assert.NotEqual(Guid.Empty, id2.Value);
            Assert.NotEqual(id1, id2);
        }
    }
}
