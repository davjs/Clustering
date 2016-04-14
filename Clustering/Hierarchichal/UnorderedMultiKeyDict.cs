using System;
using System.Collections.Generic;

namespace Clustering.Hierarchichal
{
    public class UnorderedMultiKeyDict<TKey, TValue> : Dictionary<Tuple<TKey, TKey>, TValue>
    {
        private static Tuple<TKey, TKey> GetOrderedTuple(TKey key1, TKey key2) =>
            key1.GetHashCode() > key2.GetHashCode()
                ? Tuple.Create(key1, key2)
                : Tuple.Create(key2, key1);

        public void Add(TKey key1, TKey key2, TValue value)
        {
            Add(GetOrderedTuple(key1, key2), value);
        }

        public TValue Get(TKey key1, TKey key2) =>
            this[GetOrderedTuple(key1, key2)];

        internal void Remove(TKey node, TKey item1)
        {
            Remove(GetOrderedTuple(node, item1));
        }
    }
}