using System;
using TypeRealm.Domain.Tests.Common;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class PlayerNameTests
    {
        [Fact]
        public void ShouldBePrimitive()
        {
            Assert.IsAssignableFrom<Primitive<string>>(Fixture.PlayerName());
        }

        [Fact]
        public void ShouldThrowWhenEmpty()
        {
            Assert.Throws<ArgumentException>(() => new PlayerName(string.Empty));
        }

        [Fact]
        public void ShouldThrowWhenLengthIsLessThan3()
        {
            Assert.Throws<ArgumentException>(() => new PlayerName("a"));
            Assert.Throws<ArgumentException>(() => new PlayerName("ab"));
        }

        [Fact]
        public void ShouldThrowWhenLengthIsMoreThan20()
        {
            Assert.Throws<ArgumentException>(() => new PlayerName(new string('a', 21)));
            Assert.Throws<ArgumentException>(() => new PlayerName(new string('a', 40)));
        }

        [Fact]
        public void ShouldCreateWhenLengthIsBetween3And20()
        {
            Assert.Equal("abc", new PlayerName("abc").Value);
            Assert.Equal("abcd", new PlayerName("abcd").Value);
            Assert.Equal(new string('a', 20), new PlayerName(new string('a', 20)).Value);
            Assert.Equal(new string('a', 19), new PlayerName(new string('a', 19)).Value);
        }

        [Fact]
        public void ShouldBeImplicitelyConvertible()
        {
            string value;
            var playerName = new PlayerName("player name");
            value = playerName;

            Assert.Equal("player name", value);

            PlayerName fromValue = value;
            Assert.Equal("player name", fromValue.Value);
        }
    }
}
