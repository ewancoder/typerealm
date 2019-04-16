using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class AuthorizationService : IAuthorizationService
    {
        private readonly ILogger _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILocationStore _locationStore;

        public AuthorizationService(
            ILogger logger,
            IAccountRepository accountRepository,
            IPlayerRepository playerRepository,
            ILocationStore locationStore)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _playerRepository = playerRepository;
            _locationStore = locationStore;
        }

        public PlayerId AuthorizeOrCreate(
            string login, string password, PlayerName playerName)
        {
            var account = _accountRepository.FindByLogin(login);
            if (account == null)
            {
                var accountId = _accountRepository.NextId();

                // Create a new account.
                account = new Account(accountId, login, password);

                _accountRepository.Save(account);
            }

            if (password != account.Password)
            {
                // Invalid password.
                _logger.Log($"{login} tried to connect with invalid password.");
                return null;
            }

            var player = _playerRepository.FindByName(playerName);
            if (player == null)
            {
                var playerId = _playerRepository.NextId();
                var locationId = _locationStore.GetStartingLocationId();

                // Create a new player.
                player = account.CreatePlayer(
                    playerId, playerName, locationId);

                _playerRepository.Save(player);
            }

            if (player.AccountId != account.AccountId)
            {
                _logger.Log($"{login} tried to create a player with a name that already exists on account {account.AccountId}.");
                return null;
            }

            return player.PlayerId;
        }
    }
}
