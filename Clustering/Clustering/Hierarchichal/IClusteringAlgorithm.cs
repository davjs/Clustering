using System.Collections.Generic;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public interface IClusteringAlgorithm
    {
        IEnumerable<Node> Cluster(IEnumerable<Node> Nodes);
    }
}