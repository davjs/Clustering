using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Hierarchichal.CuttingAlgorithms
{
    //TODO:
    /*class SimilarityThreshold : IcuttingAlgorithm
    {
        public IEnumerable<ClusterNode> Cut(ISet<ClusterNode> tree,double threshhold)
        {
            foreach (var clusterNode in tree)
            {
                if (clusterNode._similarity >= threshhold)
                    yield return clusterNode;
                else
                {
                    foreach (var x in Cut(clusterNode.Children,threshhold))
                        yield return x;
                }
            }
        } 
    }

    internal interface IcuttingAlgorithm
    {
    }*/
}
