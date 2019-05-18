using System;

namespace TypeRealm.Domain
{
    public sealed class RoadId : Primitive<int>
    {
        public RoadId(int value) : base(value)
        {
            if (value <= 0)
                throw new ArgumentException("Road identity should have a positive value.", nameof(value));
        }

        public static implicit operator int(RoadId roadId)
            => roadId.Value;
    }
}
