using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class AccountTests
    {
        [Fact]
        public void ShouldCreate()
        {
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "login", "password");

            Assert.Equal(accountId, account.AccountId);
            Assert.Equal("login", account.Login);
            Assert.Equal("password", account.Password);
        }

        [Fact]
        public void ShouldCreatePlayer()
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
