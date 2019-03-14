using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeRealm.Messages
{
    /// <summary>
    /// Indexed collection, index starts from 0.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    internal sealed class IndexedCollection<TValue>
    {
        private readonly Dictionary<int, TValue> _indexToValue;
        private readonly Dictionary<TValue, int> _valueToIndex;

        public IndexedCollection(IEnumerable<TValue> values)
        {
            _indexToValue = values
                .Select((x, i) => new { Index = i, Value = x })
                .ToDictionary(x => x.Index, x => x.Value);

            _valueToIndex = _indexToValue.ToDictionary(x => x.Value, x => x.Key);
        }

        public TValue GetValue(int index)
        {
            if (!_indexToValue.ContainsKey(index))
                throw new InvalidOperationException("Indexed item does not exist.");

            return _indexToValue[index];
        }

        public int GetIndex(TValue value)
        {
            if (!_valueToIndex.ContainsKey(value))
                throw new InvalidOperationException("Indexed item does not exist.");

            return _valueToIndex[value];
        }
    }
}
