using System;

namespace TypeRealm.Domain
{
    public sealed class Account
    {
        public Account(Guid accountId, string login, string password)
        {
            AccountId = accountId;
            Login = login;
            Password = password;
        }

        public Guid AccountId { get; }
        public string Login { get; }
        public string Password { get; }

        public Player CreatePlayer(Guid playerId, string name)
        {
            return new Player(playerId, AccountId, name);
        }
    }
}
