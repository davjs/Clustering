using System.Collections.Generic;
using Clustering.SolutionModel.Nodes;

namespace Clustering.SimilarityMetrics
{
    public interface ISimilarityMectric
    {
        double Calc(IEnumerable<Node> created, IEnumerable<Node> groundTruth);
    }
}
