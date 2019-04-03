﻿using System;
using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class AuthorizationService : IAuthorizationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger _logger;

        public AuthorizationService(
            IAccountRepository accountRepository,
            IPlayerRepository playerRepository,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public PlayerId AuthorizeOrCreate(
            string login, string password, string playerName)
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

            var player = _playerRepository.FindByName(account.AccountId, playerName);
            if (player == null)
            {
                var playerId = _playerRepository.NextId();

                // Create a new player.
                player = account.CreatePlayer(
                    playerId, playerName);

                _playerRepository.Save(player);
            }

            return player.PlayerId;
        }
    }
}
