using System;

namespace TypeRealm.Domain
{
    public sealed class LocationId : Primitive<int>
    {
        public LocationId(int value) : base(value)
        {
            if (value <= 0)
                throw new ArgumentException("Location identity should have a positive value.", nameof(value));
        }
    }
}
