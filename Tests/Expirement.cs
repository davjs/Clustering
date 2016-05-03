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
        private readonly Dictionary<string, string> _solutionPaths =
        new Dictionary<string, string>
        {
            {"MonoGame","MonoGame.Framework.Windows.sln"}
        };
        private static string ParsedRepoLocation(string repoName) =>
            $@"{LocalPathConfig.ParsedDataLocation}\{repoName}\";

        public void PrepareExperiment(string repoName)
        {
            BenchMark.Prepare($@"{LocalPathConfig.RepoLocations}\{repoName}\{_solutionPaths[repoName]}", 
                ParsedRepoLocation(repoName));
        }


        public List<BenchMarkResult> RunBenchMark(string repoName)
        {
            var benchmark = new Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>();
            return benchmark.RunAllInFolder(ParsedRepoLocation(repoName)).ToList();
        }

        // TESTS
        
        [TestMethod]
        public void PrepareMonoData()
        {
            PrepareExperiment("MonoGame");
        }

        [TestMethod]
        public void RunBenchmarkTest()
        {
            var result = RunBenchMark("MonoGame");
        }
    }

}
