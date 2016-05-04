using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;
using Clustering.SolutionModel.Serializing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Benchmark
    {
        private readonly Dictionary<string, Repository> _repositories =
            new Dictionary<string, Repository>
            {
                {"MonoGame", new Repository("MonoGame", "Mono", "MonoGame.Framework.Windows.sln") },
                {"octokit.net", new Repository("octokit.net", "octokit", "Octokit.sln") }
            };
        private static string ParsedRepoLocation(Repository repo) =>
            $@"{LocalPathConfig.ParsedDataLocation}\{repo.Owner}\{repo.Name}\";

        public void PrepareExperiment(string repoName)
        {
            var repo = _repositories[repoName];
            BenchMark.Prepare($@"{LocalPathConfig.RepoLocations}\{repo.Name}\{repo.Solution}", 
                ParsedRepoLocation(repo));
        }
        
        public List<BenchMarkResult> RunBenchMark(string repoName)
        {
            var benchmark = new Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>();
            return benchmark.RunAllInFolder(ParsedRepoLocation(_repositories[repoName])).ToList();
        }

        // TESTS
        
        [TestMethod]
        public void PrepareData()
        {
            PrepareExperiment("octokit.net");
        }

        [TestMethod]
        public void RunBenchmarkTest()
        {
            var result = RunBenchMark("octokit.net");
        }
    }

}
