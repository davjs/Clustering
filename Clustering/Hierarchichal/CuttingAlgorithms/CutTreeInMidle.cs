using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal.CuttingAlgorithms
{
    public class CutTreeInMidle : ICuttingAlgorithm
    {
        public IEnumerable<Node> Cut(ISet<Node> tree)
            => CutTreeAtDepth(tree, tree.Height() / 2);
        
        public static IEnumerable<Node> CutTreeAtDepth(IEnumerable<Node> nodes, int depth)
        {
            var isLeaf = nodes.ToLookup(x => !(x is ClusterNode));
            var leafNodes = isLeaf[true].Select(x => new SingletonCluster(x) as Node);

            if (depth == 0)
                return leafNodes.Union(isLeaf[false].Select(x => new NamedNode(x.Name, x.LeafNodes())));

            var flattenedChildren = isLeaf[false].SelectMany(node => CutTreeAtDepth(node.Children, depth - 1));

            return leafNodes.Union(flattenedChildren).ToSet();
        }
    }
}