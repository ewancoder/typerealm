using Moq;
using System;
using TypeRealm.Domain;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly AuthorizationService _sut;

        public AuthorizationServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _sut = new AuthorizationService(
                _accountRepositoryMock.Object, _playerRepositoryMock.Object, new Mock<ILogger>().Object);
        }

        [Fact]
        public void ShouldCreateAccountWhenNotExists()
        {
            var accountId = AccountId.New();
            _accountRepositoryMock
                .Setup(x => x.NextId())
                .Returns(accountId);

            _sut.AuthorizeOrCreate("login", "password", "playerName");

            _accountRepositoryMock.Verify(x => x.Save(It.Is<Account>(
                a => a.Login == "login"
                && a.Password == "password"
                && a.AccountId == accountId)));
        }

        [Fact]
        public void ShouldNotAuthorizeWhenPasswordIsWrong()
        {
            _accountRepositoryMock
                .Setup(x => x.FindByLogin("login"))
                .Returns(new Account(AccountId.New(), "login", "another-password"));

            var playerId = _sut.AuthorizeOrCreate("login", "password", "playerName");

            Assert.Null(playerId);
        }

        [Fact]
        public void ShouldCreatePlayerWhenNotExists()
        {
            var accountId = AccountId.New();
            var playerId = PlayerId.New();

            _accountRepositoryMock
                .Setup(x => x.FindByLogin("login"))
                .Returns(new Account(accountId, "login", "password"));

            _playerRepositoryMock
                .Setup(x => x.NextId())
                .Returns(playerId);

            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", "playerName");

            _playerRepositoryMock.Verify(x => x.Save(It.Is<Player>(
                p => p.PlayerId == playerId
                && p.AccountId == accountId
                && p.Name == "playerName")));

            Assert.Equal(playerId, authorizedPlayerId);
        }

        [Fact]
        public void ShouldNotAuthorizePlayerFromAnotherAccount()
        {
            var accountId = AccountId.New();
            var playerId = PlayerId.New();

            var anotherAccount = new Account(AccountId.New(), "l", "p");

            _accountRepositoryMock
                .Setup(x => x.FindByLogin("login"))
                .Returns(new Account(accountId, "login", "password"));

            _playerRepositoryMock
                .Setup(x => x.FindByName("playerName"))
                .Returns(anotherAccount.CreatePlayer(playerId, "playerName"));

            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", "playerName");

            Assert.Null(authorizedPlayerId);
        }

        [Fact]
        public void ShouldAuthorizePlayer()
        {
            var accountId = AccountId.New();
            var playerId = PlayerId.New();

            var account = new Account(accountId, "login", "password");

            _accountRepositoryMock
                .Setup(x => x.FindByLogin("login"))
                .Returns(account);

            _playerRepositoryMock
                .Setup(x => x.FindByName("playerName"))
                .Returns(account.CreatePlayer(playerId, "playerName"));

            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", "playerName");

            Assert.Equal(playerId, authorizedPlayerId);
        }
    }
}
