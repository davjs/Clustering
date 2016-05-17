using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;

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

        public static IEnumerable<Node> LeafNodes(this Node me)
        {
            foreach (var child in me.Children)
            {
                if (child.IsLeafNode())
                    yield return child;
                else
                {
                    foreach (var classNode in child.LeafNodes())
                        yield return classNode;
                }
            }
        }
        
        public static IEnumerable<Node> LeafClusters(this IEnumerable<Node> nodes)
        {
            var clusters = nodes.Where(x => x.Children.Any());
            var isLeaf = clusters.ToLookup(x => x.IsLeafCluster());
            return isLeaf[true]
                .Union(
                    isLeaf[false].SelectMany(x => x.Children.LeafClusters())
                );
        }

        private static bool IsLeafCluster(this Node node) 
            => node.Children.All(x => x.IsLeafNode());

        private static bool IsLeafNode(this Node node)
            => node.Children.Count == 0;

        public static int Height(this ISet<Node> nodes)
        {
            if (!nodes.Any())
                return 0;
            return nodes.Max(x => Height(x.Children)) + 1;
        }
    }

    public static class Extensions
    {
        public static ISet<T> ToSet<T>(this IEnumerable<T> enumerable) =>
            new HashSet<T>(enumerable);
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
