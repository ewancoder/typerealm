using System;

namespace TypeRealm.Domain
{
    public sealed class PlayerId : Primitive<Guid>
    {
        public PlayerId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Player identity cannot be empty.");
        }

        public static PlayerId New()
        {
            return new PlayerId(Guid.NewGuid());
        }
    }
}
