using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Clustering;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SimilarityMetrics.MojoFM;
using Tests.Building.TestExtensions;

namespace Tests
{
    public class SolutionBenchmark
    {
        public class WeightedCombinedStatic : Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>{}

        private static string ParsedRepoLocation(Repository repo) =>
            $@"{LocalPathConfig.ParsedDataLocation}\{repo.Owner}\{repo.Name}\";
        private static string RepositoryLocation(Repository repo) =>
            $@"{LocalPathConfig.RepoLocations}\{repo.Name}\{repo.Solution}";

        public static void RunAllInFolder<TClusterAlg, TCuttingAlg, TMetric>
            (Benchmark<TClusterAlg, TCuttingAlg, TMetric> benchMarkConfig,Repository repo)
            where TClusterAlg : ClusteringAlgorithm, new()
            where TCuttingAlg : ICuttingAlgorithm, new()
            where TMetric : ISimilarityMectric, new()
        {
            var dataFolder = ParsedRepoLocation(repo);
            var outputFolder = Paths.SolutionFolder + @"BenchMarkResults\";
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
