using System;
using System.Collections.Generic;
using System.Linq;
using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class InMemoryAccountRepository : IAccountRepository
    {
        private readonly List<Account> _accounts = new List<Account>();

        public Account FindByLogin(string login)
        {
            return _accounts.SingleOrDefault(a => a.Login == login);
        }

        public AccountId NextId()
        {
            return AccountId.New();
        }

        public void Save(Account account)
        {
            if (_accounts.Contains(account))
                return;

            if (_accounts.Any(a => a.AccountId == account.AccountId)
                || _accounts.Any(a => a.Login == account.Login))
                throw new InvalidOperationException("The account already exists.");

            _accounts.Add(account);
        }
    }
}
