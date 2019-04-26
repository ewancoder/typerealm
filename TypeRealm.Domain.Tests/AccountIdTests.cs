using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class AccountIdTests
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
    }
}
