using System;

namespace TypeRealm.Domain
{
    public sealed class Distance : Primitive<int>
    {
        public Distance(int value) : base(value)
        {
            if (value < 0)
                throw new ArgumentException("Distance cannot have negative value.", nameof(value));
        }

        public Distance Add(Distance distance)
            => new Distance(Value + distance.Value);

        public bool IsGreaterThan(Distance distance)
            => Value > distance.Value;

        public bool IsZero()
        {
            return Value == 0;
        }

        public static Distance Zero => new Distance(0);

        public static implicit operator Distance(int value)
            => new Distance(value);
    }
}
