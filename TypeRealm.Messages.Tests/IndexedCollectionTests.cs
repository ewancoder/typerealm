using System;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public class IndexedCollectionTests
    {
        [Fact]
        public void ShouldGetValue()
        {
            var array = new string[] { "a", "b", "c" };
            var sut = new IndexedCollection<string>(array);

            Assert.Equal("a", sut.GetValue(0));
            Assert.Equal("b", sut.GetValue(1));
            Assert.Equal("c", sut.GetValue(2));
        }

        [Fact]
        public void ShouldGetIndex()
        {
            var array = new string[] { "a", "b", "c" };
            var sut = new IndexedCollection<string>(array);

            Assert.Equal(0, sut.GetIndex("a"));
            Assert.Equal(1, sut.GetIndex("b"));
            Assert.Equal(2, sut.GetIndex("c"));
        }

        [Fact]
        public void ShouldThrowWhenNoValue()
        {
            var array = new string[] { "a", "b", "c" };
            var sut = new IndexedCollection<string>(array);

            Assert.Throws<InvalidOperationException>(() => sut.GetValue(-1));
            Assert.Throws<InvalidOperationException>(() => sut.GetValue(3));

            Assert.Throws<ArgumentNullException>(() => sut.GetIndex(null));
            Assert.Throws<InvalidOperationException>(() => sut.GetIndex(string.Empty));
            Assert.Throws<InvalidOperationException>(() => sut.GetIndex("non-existent"));
        }

        [Fact]
        public void ShouldThrowWhenDuplicateValue()
        {
            var array = new string[] { "a", "b", "a", "c" };
            Assert.Throws<ArgumentException>(() => new IndexedCollection<string>(array));
        }
    }
}
