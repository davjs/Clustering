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
    /*
        -- PREPARE EXPERIMENT EXAMPLE --

        [TestMethod]
        public void Experiment1()
        {
            BenchMark.Prepare("F:\\Experiments\\SignalR\\Microsoft.AspNet.SignalR.sln",
                "F:\\Experiments\\Output\\");
        }
    */

        [TestMethod]
        public void RunBenchmark()
        {
            var benchmark = new Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>();
            var project = GraphDecoder.Decode(File.ReadAllText(@"C:\parsed-csharp-repos\SignalR\Microsoft.AspNet.SignalR.Tests.Common"));
            var results = benchmark.Run(project);
        }
    }

}
