using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;

namespace Clustering.Benchmarking
{
    public class BenchmarkConfig<TClusterAlg, TCuttingAlg, TMetric> : IBenchmarkConfig
        where TClusterAlg : ClusteringAlgorithm, new()
        where TCuttingAlg : ICuttingAlgorithm, new()
        where TMetric : ISimilarityMectric, new()
    {
        public BenchmarkConfig(string name) {Name = name;}
        public ISimilarityMectric SimilarityMectric { get; } = new TMetric();
        public ICuttingAlgorithm CuttingAlgorithm { get; } = new TCuttingAlg();
        public ClusteringAlgorithm ClusteringAlgorithm { get; } = new TClusterAlg();
        public string Name { get; }
    }
}