using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ShouldCreatePlayer()
        {
            var playerId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var player = new Player(playerId, accountId, "name");

            Assert.Equal(accountId, player.AccountId);
            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal("name", player.Name);
        }
    }
}
