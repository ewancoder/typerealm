using System;

namespace TypeRealm.Domain.Tests
{
    internal static class Fixture
    {
        public static Account Account()
        {
            return new Account(AccountId.New(), "login", "password");
        }
    }
}
