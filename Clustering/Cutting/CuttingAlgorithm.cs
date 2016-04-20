using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Clustering.Hierarchichal;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Cutting
{
    public class CuttingAlgorithm
    {
        public static IEnumerable<Node> Cut(IEnumerable<Node> tree)
        {
            var depth = 0;
            foreach (var node in tree)
            {
                depth = Math.Max(Depth(node, 0), depth);
            }

            depth /= 2;

            return GetNodes(tree, depth);
        }

        private static int Depth(Node node, int depth)
        {
            //return node.Children.Max(x => Depth(x, depth + 1));
            var maxdepth = 0;
            foreach (var c in node.Children)
            {
                maxdepth = Math.Max(Depth(c, depth + 1), maxdepth);
            }
            return maxdepth;
        }

        public static IEnumerable<Node> GetNodes(IEnumerable<Node> nodes, int depth)
        {
            if (depth == 0)
                foreach (var x in nodes.Select(x => new NamedNode("$", x.Decendants())))
                    yield return x;
            
            foreach (var node in nodes)
            {
                var clusterFound = GetNodes(node.Children.OfType<NamedNode>().ToSet(), depth - 1);
                foreach (var flatClusterNode in clusterFound)
                    yield return flatClusterNode;
            }
        }
    }
}
