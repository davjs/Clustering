using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SimilarityMetrics.MojoFM;

namespace Clustering.Benchmarking
{
    public class BenchmarkConfig<TClusterAlg, TCuttingAlg, TMetric> : IBenchmarkConfig
        where TClusterAlg : ClusteringAlgorithm, new()
        where TCuttingAlg : ICuttingAlgorithm, new()
        where TMetric : ISimilarityMectric, new()
    {
        protected BenchmarkConfig(string name) {Name = name;}
        public ISimilarityMectric SimilarityMectric { get; } = new TMetric();
        public ICuttingAlgorithm CuttingAlgorithm { get; } = new TCuttingAlg();
        public ClusteringAlgorithm ClusteringAlgorithm { get; } = new TClusterAlg();
        public string Name { get; set; }
        public IBenchmarkConfig Clone() => new BenchmarkConfig<TClusterAlg, TCuttingAlg, TMetric>(Name);
    }

    public class MojoHalfBenchmark<TClusterAlg>
        : BenchmarkConfig<TClusterAlg,CutTreeInMidle,MojoFM>
        where TClusterAlg : ClusteringAlgorithm, new()
    {
        public MojoHalfBenchmark(string name) : base(name)
        {
        }
    }
}