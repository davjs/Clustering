using System;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public interface ISimilarityMatrix
    {
        Tuple<Node, Node> GetMostSimilar();
    }
}