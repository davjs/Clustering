using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering;
using Clustering.Benchmarking;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;
using Clustering.SolutionModel.Serializing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building.TestExtensions;

namespace Tests
{
    [TestClass]
    public class Benchmark
    {

        private readonly Dictionary<string, Repository> _repositories =
            new Dictionary<string, Repository>
        {
                {"MonoGame", new Repository("MonoGame", "Mono", "MonoGame.Framework.Windows.sln") },
                {"octokit.net", new Repository("octokit.net", "octokit", "Octokit.sln") },
                {"DotNetOpenAuth", new Repository("DotNetOpenAuth", "DotNetOpenAuth", "src\\DotNetOpenAuth.sln") },
                {"SignalR", new Repository("SignalR", "SignalR", "Microsoft.AspNet.SignalR.sln") }
        };

        private string currentRepoToTest = "Fail";

        // Data we now for sure is correct and has been parsed after all parse related bugs where resolved
        private readonly IEnumerable<string> _availibleParsedData = new List<string>()
        {
            "MonoGame","DotNetOpenAuth","SignalR"
        };

        // TESTS
        [TestMethod]
        public void PrepareData()
        {
            SolutionBenchmark.Prepare(_repositories[currentRepoToTest]);
        }

        [TestMethod]
        public void RunSpecificBenchmark()
        {
            var markConfig = new SolutionBenchmark.WeightedCombinedStaticMojoFM();
            SolutionBenchmark.RunAllInFolder(new List<IBenchmarkConfig> {markConfig}, _repositories[currentRepoToTest]);
        }

        [TestMethod]
        public void BenchAllAvailibleData()
        {
            var markConfig = new SolutionBenchmark.WeightedCombinedStaticMojoFM();
            foreach (var repo in _availibleParsedData)
            {
                SolutionBenchmark.RunAllInFolder(
                    new List<IBenchmarkConfig> { markConfig },
                    _repositories[repo]);
            }
        }
    }

}
