﻿using System;
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

        private string currentRepoToTest = "SignalR";

        // TESTS
        [TestMethod]
        public void PrepareData()
        {
            SolutionBenchmark.Prepare(_repositories[currentRepoToTest]);
        }

        [TestMethod]
        public void RunBenchmarkTest()
        {
            var markConfig = new SolutionBenchmark.WeightedCombinedStaticMojoFM();
            SolutionBenchmark.RunAllInFolder(markConfig,_repositories[currentRepoToTest]);
        }
    }

}
