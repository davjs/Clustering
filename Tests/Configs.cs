using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.Hierarchichal.DirectLinks;
using Clustering.SimilarityMetrics.MojoFM;

namespace Tests
{
    public class WeightedCombinedDepOnly : MojoHalfBenchmark<SiblingLinkWeightedCombinedDepOnly>{
        public WeightedCombinedDepOnly() : base("WCAD-Halfcut")
        {
        }
    }

    public class WeightedCombinedUsageOnly : MojoHalfBenchmark<SiblingLinkWeightedCombinedUsageOnly>
    {
        public WeightedCombinedUsageOnly() : base("WCAUO-Halfcut")
        {
        }
    }

    public class WeightedCombinedSymmetricHalfMojoFm : MojoHalfBenchmark<SiblingLinkWeightedCombinedSymetric>
    {
        public WeightedCombinedSymmetricHalfMojoFm() : base("WCAS-Halfcut")
        {
        }
    }
    public class WeightedCombinedSepUsage : MojoHalfBenchmark<SiblingLinkWeightedCombinedSepUsage>
    {
        public WeightedCombinedSepUsage() : base("WCASU-Halfcut")
        {
        }
    }


    public class WeightedCombinedSymmetricThreshMojoFm : MojoHalfBenchmark<SiblingLinkWeightedCombinedSymetric>
    {
        public WeightedCombinedSymmetricThreshMojoFm(double thresh) : base($"WCAS-SimCut({thresh})")
        {
        }
    }

    public class WeightedCombinedDepOnlyWithDirectLink :
        MojoHalfBenchmark<SiblingLinkWeightedCombinedDepOnlyDirectLink>
    {
        public WeightedCombinedDepOnlyWithDirectLink() : base("WCAD.DL-Halfcut")
        {
        }
    }

    public class WeightedCombinedSepusageDirectLink : BenchmarkConfig<SiblingLinkWeightedCombinedSepUsageDirectLink, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSepusageDirectLink() : base("WCASU.DL-halfcut")
        {
        }
    }
}