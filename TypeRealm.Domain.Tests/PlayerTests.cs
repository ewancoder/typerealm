﻿using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ShouldCreatePlayer()
        {
            var playerId = Fixture.PlayerId();
            var accountId = Fixture.AccountId();
            var playerName = Fixture.PlayerName();
            var locationId = Fixture.LocationId();

            var player = new Player(playerId, accountId, playerName, locationId);

            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal(accountId, player.AccountId);
            Assert.Equal(playerName, player.Name);
            Assert.Equal(locationId, player.LocationId);
        }
    }
}
