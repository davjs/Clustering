using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;

namespace Tests
{
    public class WeightedCombinedStaticMojoFm : BenchmarkConfig<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>{
        public WeightedCombinedStaticMojoFm() : base("WCA-Halfcut-MFM")
        {
        }
    }

    public class WeightedCombinedSymmetricHalfMojoFm : BenchmarkConfig<SiblingLinkWeightedCombinedSymetric, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSymmetricHalfMojoFm() : base("WCAS-Halfcut-MFM")
        {
        }
    }
    public class WeightedCombinedSepUsage : BenchmarkConfig<SiblingLinkWeightedCombinedSepUsage, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSepUsage() : base("WCAUS-Halfcut-MFM")
        {
        }
    }

    public class WeightedCombinedSymmetricThreshMojoFm : BenchmarkConfig<SiblingLinkWeightedCombinedSymetric, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSymmetricThreshMojoFm(double thresh) : base($"WCAS-SimCut-MFM({thresh})")
        {
        }
    }
}