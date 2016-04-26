using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal.CuttingAlgorithms
{
    class SimilarityThreshold : ICuttingAlgorithm
    {
        private readonly double _threshhold;

        public SimilarityThreshold(double threshhold)
        {
            _threshhold = threshhold;
        }

        public IEnumerable<Node> Cut(ISet<Node> tree)
        {
            var clusterNodes = tree.OfType<ClusterNode>();
            var isAboveThreshold = clusterNodes.ToLookup(x => x._similarity >= _threshhold);
            return isAboveThreshold[true].Select(x => new NamedNode("$", x.LeafNodes()))
                .Union(isAboveThreshold[false].SelectMany(x => Cut(x.Children)));
        } 
    }
}
