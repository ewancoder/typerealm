using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ShouldCreateFromAccount()
        {
            var account = Fixture.Account();
            var playerId = Guid.NewGuid();

            var player = account.CreatePlayer(playerId, "name");

            Assert.Equal(account.AccountId, player.AccountId);
            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal("name", player.Name);
        }
    }
}
