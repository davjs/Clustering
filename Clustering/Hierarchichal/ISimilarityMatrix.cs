using System;
using System.Collections.Generic;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public interface ISimilarityMatrix
    {
        KeyValuePair<Tuple<Node, Node>, double> GetMostSimilar();
    }
}