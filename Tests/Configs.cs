using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.Hierarchichal.DirectLinks;
using Clustering.SimilarityMetrics.MojoFM;

namespace Tests
{
    public class WeightedCombinedDepOnly : BenchmarkConfig<SiblingLinkWeightedCombinedDepOnly, CutTreeInMidle, MojoFM>{
        public WeightedCombinedDepOnly() : base("WCAD-Halfcut")
        {
        }
    }

    public class WeightedCombinedUsageOnly :
        BenchmarkConfig<SiblingLinkWeightedCombinedUsageOnly, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedUsageOnly() : base("WCAUO-Halfcut")
        {
        }
    }

    public class WeightedCombinedSymmetricHalfMojoFm : BenchmarkConfig<SiblingLinkWeightedCombinedSymetric, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSymmetricHalfMojoFm() : base("WCAS-Halfcut")
        {
        }
    }
    public class WeightedCombinedSepUsage : BenchmarkConfig<SiblingLinkWeightedCombinedSepUsage, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSepUsage() : base("WCASU-Halfcut")
        {
        }
    }


    public class WeightedCombinedSymmetricThreshMojoFm : BenchmarkConfig<SiblingLinkWeightedCombinedSymetric, CutTreeInMidle, MojoFM>
    {
        public WeightedCombinedSymmetricThreshMojoFm(double thresh) : base($"WCAS-SimCut({thresh})")
        {
        }
    }

    public class WeightedCombinedDepOnlyWithDirectLink :
        BenchmarkConfig<SiblingLinkWeightedCombinedDepOnlyDirectLink, CutTreeInMidle, MojoFM>
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