using Moq;
using TypeRealm.Domain;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<ILocationStore> _locationStoreMock;
        private readonly AuthorizationService _sut;

        public AuthorizationServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _locationStoreMock = new Mock<ILocationStore>();
            _sut = new AuthorizationService(
                new Mock<ILogger>().Object,
                _accountRepositoryMock.Object,
                _playerRepositoryMock.Object,
                _locationStoreMock.Object);
        }

        [Fact]
        public void ShouldCreateAccountWhenNotExists()
        {
            var accountId = AccountId.New();
            _accountRepositoryMock
                .Setup(x => x.NextId())
                .Returns(accountId);

            _sut.AuthorizeOrCreate("login", "password", Fixture.PlayerName());

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

            var playerId = _sut.AuthorizeOrCreate("login", "password", Fixture.PlayerName());

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

            var locationId = Fixture.LocationId();
            _locationStoreMock
                .Setup(x => x.GetStartingLocationId())
                .Returns(locationId);

            var playerName = Fixture.PlayerName();
            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", playerName);

            _playerRepositoryMock.Verify(x => x.Save(It.Is<Player>(
                p => p.PlayerId == playerId
                && p.AccountId == accountId
                && p.Name == playerName
                && p.LocationId == locationId)));

            Assert.Equal(playerId, authorizedPlayerId);
        }


        [Fact]
        public void ShouldCreatePlayerAndRegisterAccountForNewcomers()
        {
            var accountId = AccountId.New();
            var playerId = PlayerId.New();

            _accountRepositoryMock
                .Setup(x => x.NextId())
                .Returns(accountId);

            _playerRepositoryMock
                .Setup(x => x.NextId())
                .Returns(playerId);

            var locationId = Fixture.LocationId();
            _locationStoreMock
                .Setup(x => x.GetStartingLocationId())
                .Returns(locationId);

            var playerName = Fixture.PlayerName();
            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", playerName);

            _accountRepositoryMock.Verify(x => x.Save(It.Is<Account>(
                a => a.Login == "login"
                && a.Password == "password"
                && a.AccountId == accountId)));

            _playerRepositoryMock.Verify(x => x.Save(It.Is<Player>(
                p => p.PlayerId == playerId
                && p.AccountId == accountId
                && p.Name == playerName
                && p.LocationId == locationId)));

            Assert.Equal(playerId, authorizedPlayerId);
        }

        [Fact]
        public void ShouldNotAuthorizePlayerThatAlreadyExistsOnAnotherAccount()
        {
            var accountId = AccountId.New();
            var playerId = PlayerId.New();

            var anotherAccount = new Account(AccountId.New(), "l", "p");

            _accountRepositoryMock
                .Setup(x => x.FindByLogin("login"))
                .Returns(new Account(accountId, "login", "password"));

            var playerName = Fixture.PlayerName();
            var locationId = Fixture.LocationId();

            _playerRepositoryMock
                .Setup(x => x.FindByName(playerName))
                .Returns(anotherAccount.CreatePlayer(playerId, playerName, locationId));

            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", playerName);

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

            var playerName = Fixture.PlayerName();
            var locationId = Fixture.LocationId();

            _playerRepositoryMock
                .Setup(x => x.FindByName(playerName))
                .Returns(account.CreatePlayer(playerId, playerName, locationId));

            var authorizedPlayerId = _sut.AuthorizeOrCreate("login", "password", playerName);

            Assert.Equal(playerId, authorizedPlayerId);
        }
    }
}
