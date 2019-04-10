using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ShouldCreatePlayer()
        {
            var playerId = PlayerId.New();
            var accountId = AccountId.New();

            var playerName = Fixture.PlayerName();
            var player = new Player(playerId, accountId, playerName);

            Assert.Equal(accountId, player.AccountId);
            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal(playerName, player.Name);
        }
    }
}
