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

        public static implicit operator int(LocationId locationId)
            => locationId.Value;

        public static implicit operator LocationId(int value)
            => new LocationId(value);
    }
}
