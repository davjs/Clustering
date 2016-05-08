using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal.CuttingAlgorithms
{
    public class SimilarityThreshold : ICuttingAlgorithm
    {
        public double Threshhold;

        public SimilarityThreshold()
        {
            Threshhold = 0.05;
        }

        public IEnumerable<Node> Cut(ISet<Node> nodes)
        {
            var isLeaf = nodes.ToLookup(x => !(x is ClusterNode));
            var leafNodes = isLeaf[true].Select(x => new SingletonCluster(x) as Node);
            
            var isAboveThreshold = isLeaf[false].Cast<ClusterNode>()
                .ToLookup(x => x._similarity >= Threshhold);

            return isAboveThreshold[true].Select(x => new NamedNode("$", x.LeafNodes()))
                .Union(isAboveThreshold[false].SelectMany(x => Cut(x.Children))).Union(leafNodes);
        } 
    }
}
