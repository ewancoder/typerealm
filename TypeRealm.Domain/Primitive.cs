using System;
using System.Collections.Generic;

namespace TypeRealm.Domain
{
    public abstract class Primitive<TValue>
    {
        protected Primitive(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public TValue Value { get; }

        public sealed override bool Equals(object obj)
        {
            return obj is Primitive<TValue> other
                && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public sealed override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
