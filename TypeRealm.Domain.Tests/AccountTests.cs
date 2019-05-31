using TypeRealm.Domain.Tests.Common;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public sealed class AccountTests
    {
        [Fact]
        public void ShouldCreate()
        {
            var accountId = Fixture.AccountId();
            var account = new Account(accountId, "login", "password");

            Assert.Equal(accountId, account.AccountId);
            Assert.Equal("login", account.Login);
            Assert.Equal("password", account.Password);
        }

        [Fact]
        public void ShouldCreatePlayer()
        {
            var account = Fixture.Account();

            var playerId = Fixture.PlayerId();
            var playerName = Fixture.PlayerName();
            var locationId = Fixture.LocationId();

            var player = account.CreatePlayer(playerId, playerName, locationId);

            Assert.Equal(account.AccountId, player.AccountId);
            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal(playerName, player.Name);
            Assert.Equal(locationId, player.LocationId);
            Assert.Null(player.MovementInformation);
        }
    }
}
