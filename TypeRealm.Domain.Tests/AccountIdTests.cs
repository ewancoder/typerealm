using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class AccountIdTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<Guid>>(AccountId.New());
        }

        [Fact]
        public void ShouldThrowWhenEmpty()
        {
            Assert.Throws<ArgumentException>(() => new AccountId(Guid.Empty));
        }

        [Fact]
        public void ShouldCreateUnique()
        {
            var id1 = AccountId.New();
            var id2 = AccountId.New();

            Assert.NotEqual(Guid.Empty, id1.Value);
            Assert.NotEqual(Guid.Empty, id2.Value);
            Assert.NotEqual(id1, id2);
        }
    }
}
