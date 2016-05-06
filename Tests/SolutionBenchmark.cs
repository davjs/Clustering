using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Clustering;
using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;
using Clustering.SolutionModel.Serializing;
using Tests.Building.TestExtensions;

namespace Tests
{
    public class SolutionBenchmark
    {
        public class WeightedCombinedStaticMojoFM : BenchmarkConfig<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>{
            public WeightedCombinedStaticMojoFM() : base("WCA-Halfcut-MojoFM")
            {
            }
        }

        private static string ParsedRepoLocation(Repository repo) =>
            $@"{LocalPathConfig.ParsedDataLocation}\{repo.Owner}\{repo.Name}\";
        private static string RepositoryLocation(Repository repo) =>
            $@"{LocalPathConfig.RepoLocations}\{repo.Name}\{repo.Solution}";

        public static void RunAllInFolder(IEnumerable<IBenchmarkConfig> benchMarkConfigs,Repository repo)
        {
            var dataFolder = ParsedRepoLocation(repo);
            var outputFolder = Paths.SolutionFolder + @"BenchMarkResults\";
            var outputFile = outputFolder + $"{repo.Name}.Results";
            var projectGraphsInFolder = BenchMark.GetProjectGraphsInFolder(dataFolder);

            var text = new List<string>();

            foreach (var benchMarkConfig in benchMarkConfigs)
            {
                var benchMarkResults = BenchMark.RunAllInFolder(benchMarkConfig, projectGraphsInFolder);
                var thisConfigResults = benchMarkResults.Select(x => x.ToString());
                text.Add($"  -- {benchMarkConfig.Name} -- :");
                text.AddRange(thisConfigResults);
            }
            
            Directory.CreateDirectory(outputFolder);
            File.WriteAllLines(outputFile, text);
        }

        public static void Prepare(Repository repo)
        {
            BenchMark.Prepare(RepositoryLocation(repo), ParsedRepoLocation(repo));
        }

    }
}
