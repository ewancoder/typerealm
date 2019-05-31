using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class PrimitiveTests
    {
        private sealed class TestReferencePrimitive : Primitive<string>
        {
            public TestReferencePrimitive(string value) : base(value)
            {
            }
        }

        private sealed class TestValuePrimitive : Primitive<int>
        {
            public TestValuePrimitive(int value) : base(value)
            {
            }
        }

        private sealed class AnotherPrimitive : Primitive<int>
        {
            public AnotherPrimitive(int value) : base(value)
            {
            }
        }

        [Fact]
        public void TwoDifferentTypesShouldNotBeEqual()
        {
            Assert.False(new AnotherPrimitive(1).Equals(new TestValuePrimitive(1)));
        }

        [Fact]
        public void ShouldThrowWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TestReferencePrimitive(null));
        }

        [Fact]
        public void ShouldNotThrowWhenValueIsDefault()
        {
            new TestValuePrimitive(0);
        }

        [Fact]
        public void ShouldSetSuppliedValue()
        {
            Assert.Equal(string.Empty, new TestReferencePrimitive(string.Empty).Value);
            Assert.Equal("value", new TestReferencePrimitive("value").Value);
            Assert.Equal(0, new TestValuePrimitive(0).Value);
            Assert.Equal(123, new TestValuePrimitive(123).Value);
        }

        [Fact]
        public void ShouldConvertValueToStringByDefault()
        {
            Assert.Equal("value", new TestReferencePrimitive("value").ToString());
            Assert.Equal("123", new TestValuePrimitive(123).ToString());
        }

        [Fact]
        public void ShouldCompareToNull()
        {
            var p = CreateSame();

            Assert.False(Equals(p, null));
            Assert.False(p.Equals(null));

            Assert.False(p == null);
            Assert.False(null == p);
        }

        [Fact]
        public void ShouldCompareByValue()
        {
            var p1 = CreateSame();
            var p2 = CreateSame();
            var p3 = CreateDifferent();

            Assert.True(Equals(p1, p2));
            Assert.True(p1.Equals(p2));
            Assert.True(p2.Equals(p1));

            Assert.False(Equals(p1, p3));
            Assert.False(p1.Equals(p3));
            Assert.False(p3.Equals(p1));
        }

        [Fact]
        public void ShouldCompareUsingOperator()
        {
            var p1 = CreateSame();
            var p2 = CreateSame();
            var p3 = CreateDifferent();

            Assert.True(p1 == p2);
            Assert.True(p2 == p1);
            Assert.False(p1 != p2);
            Assert.False(p2 != p1);

            Assert.False(p1 == p3);
            Assert.False(p3 == p1);
            Assert.True(p1 != p3);
            Assert.True(p3 != p1);
        }

        [Fact]
        public void ShouldHaveProperHashCodes()
        {
            var p1 = CreateSame();
            var p2 = CreateSame();
            var p3 = CreateDifferent();

            Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
            Assert.NotEqual(p1.GetHashCode(), p3.GetHashCode());

            Assert.Equal(p1.Value.GetHashCode(), p1.GetHashCode());
            Assert.Equal(p3.Value.GetHashCode(), p3.GetHashCode());
        }

        private static TestReferencePrimitive CreateSame()
        {
            return new TestReferencePrimitive("value");
        }

        private static TestReferencePrimitive CreateDifferent()
        {
            return new TestReferencePrimitive("different");
        }
    }
}
