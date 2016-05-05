using System.IO;
using System.Linq;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SimilarityMetrics.MojoFM;
using Paths;

namespace BenchMarking
{
    public class SolutionBenchmark
    {
        public class WeightedCombinedStaticMojoFM : Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM> { }

        private static string ParsedRepoLocation(Repository repo) =>
            $@"{Local.ParsedDataLocation}\{repo.Owner}\{repo.Name}\";
        private static string RepositoryLocation(Repository repo) =>
            $@"{Local.RepoLocations}\{repo.Name}\{repo.Solution}";

        public static void RunAllInFolder<TClusterAlg, TCuttingAlg, TMetric>
            (Benchmark<TClusterAlg, TCuttingAlg, TMetric> benchMarkConfig, Repository repo)
            where TClusterAlg : ClusteringAlgorithm, new()
            where TCuttingAlg : ICuttingAlgorithm, new()
            where TMetric : ISimilarityMectric, new()
        {
            var dataFolder = ParsedRepoLocation(repo);
            var outputFolder = Shared.SolutionFolder + @"BenchMarkResults\";
            var outputFile = outputFolder + $"{repo.Name}.Results";
            var benchMarkResults = benchMarkConfig.RunAllInFolder(dataFolder);

            Directory.CreateDirectory(outputFolder);
            File.WriteAllLines(outputFile, benchMarkResults.Select(x => x.ToString()));
        }

        public static void Prepare(Repository repo)
        {
            BenchMark.Prepare(RepositoryLocation(repo), ParsedRepoLocation(repo));
        }
    }
}