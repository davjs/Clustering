using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;

namespace Tests
{
    public class WeightedCombinedStaticMojoFm : BenchmarkConfig<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>{
        public WeightedCombinedStaticMojoFm() : base("WCA-Halfcut-MojoFM")
        {
        }
    }

    public class WeightedCombinedStaticSimThreshMojoFm : BenchmarkConfig<SiblingLinkWeightedCombined, SimilarityThreshold, MojoFM>
    {
        public WeightedCombinedStaticSimThreshMojoFm(double thresh) : base($"WCA-SimCut-MojoFM({thresh})")
        {
            ((SimilarityThreshold) CuttingAlgorithm).Threshhold = thresh;
        }
    }
}