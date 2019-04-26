using System;

namespace TypeRealm.Domain
{
    public sealed class AccountId : Primitive<Guid>
    {
        public AccountId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Account identity cannot be empty.", nameof(value));
        }

        public static AccountId New()
        {
            return new AccountId(Guid.NewGuid());
        }
    }
}
