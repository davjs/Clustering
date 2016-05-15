using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;

namespace Clustering.Benchmarking
{
    public interface IBenchmarkConfig
    {
        ISimilarityMectric SimilarityMectric { get; }
        ICuttingAlgorithm CuttingAlgorithm { get; }
        ClusteringAlgorithm ClusteringAlgorithm { get; }
        string Name { get; set; }
        IBenchmarkConfig Clone();
    }
}