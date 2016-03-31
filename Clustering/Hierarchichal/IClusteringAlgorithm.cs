using System.Collections.Generic;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public interface IClusteringAlgorithm
    {
        ISet<Node> Cluster(ISet<Node> nodes, ISet<DependencyLink> edges);
    }
}