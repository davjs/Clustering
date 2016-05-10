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
            new List<Repository>
            {
                new Repository("MonoGame", "mono", "MonoGame.Framework.Windows.sln"),
                new Repository("octokit.net", "octokit", "Octokit.sln"),
                new Repository("DotNetOpenAuth", "DotNetOpenAuth", @"src\DotNetOpenAuth.sln"),
                new Repository("fluentmigrator", "schambers", "FluentMigrator.sln"),
                new Repository("shadowsocks-windows", "shadowsocks", "shadowsocks-windows.sln"),
                new Repository("SignalR", "SignalR", "Microsoft.AspNet.SignalR.sln"),
                new Repository("FluentValidation", "JeremySkinner", "FluentValidation.sln"),
                new Repository("dotnet", "MiniProfiler", "MiniProfiler.sln"),
                new Repository("SparkleShare", "hbons", @"SparkleShare\Windows\SparkleShare.sln"),
                new Repository("ShareX", "ShareX", "ShareX.sln"),
                new Repository("Nancy", "NancyFX", "Nancy.sln"),
                new Repository("AutoMapper", "AutoMapper", @"src\AutoMapper.sln"),
                // new Repository("RestSharp", "restsharp", "RestSharp.sln"), // Missing Xamarin
                // new Repository("Ninject", "ninject", "Ninject.sln"), // Missing Xamarin
                new Repository("EntityFramework", "aspnet", @"EntityFramework.sln"),
            }.ToDictionary(x => x.Name, x => x);

        private string currentRepoToTest = "EntityFramework";

        // Data we now for sure is correct and has been parsed after all parse related bugs where resolved
        private readonly IEnumerable<string> _availibleParsedData = new List<string>()
        {
            "MonoGame",
            "DotNetOpenAuth",
            "SignalR",
            "fluentmigrator",
            "shadowsocks-windows",
            "FluentValidation",
            "dotnet",
            "SparkleShare",
            "ShareX",
            "Nancy",
            "AutoMapper",
            "EntityFramework",
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
            var markConfig = new WeightedCombinedStaticMojoFm();
            SolutionBenchmark.RunAllInFolder(new List<IBenchmarkConfig> {markConfig},
                new List<Repository> {_repositories[currentRepoToTest]});
        }

        [TestMethod]
        public void BenchAllAvailibleData()
        {
            SolutionBenchmark.RunAllInFolder(
                new List<IBenchmarkConfig>
                {
                    new WeightedCombinedStaticMojoFm(),
                    new WeightedCombinedSymmetricHalfMojoFm(),
                    new WeightedCombinedSepUsage()
                },
                _availibleParsedData.Select(x => _repositories[x]).ToList())
                .WriteToFolder(Paths.SolutionFolder + "BenchMarkResults\\");
        }
    }
}
