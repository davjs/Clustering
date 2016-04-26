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
        
        public static IEnumerable<NamedNode> CutTreeAtDepth(IEnumerable<Node> nodes, int depth)
        {
            if (depth == 0)
                return nodes.Select(x => new NamedNode("$", x.LeafNodes()));
            return nodes.SelectMany(node => CutTreeAtDepth(node.Children, depth - 1));
        }
    }
}