using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.SolutionModel
{
    public static class NodeExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> list,T item) =>
            list.Except(new HashSet<T>{item});
        
        public static IEnumerable<T> Union<T>(this IEnumerable<T> list, T item) =>
            list.Union(new HashSet<T> { item });

        public static T WithName<T>(this IEnumerable<T> nodeList, string name) where T : Nodes.Node =>
            nodeList.FirstOrDefault(x => x.Name == name);
    }

    public static class Extensions
    {
        public static ISet<T> ToSet<T>(this IEnumerable<T> enumerable) =>
            enumerable.ToImmutableHashSet();
        public static HashSet<T> ToMutableSet<T>(this IEnumerable<T> enumerable) =>
            new HashSet<T>(enumerable);

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
            TKey key,
             TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
