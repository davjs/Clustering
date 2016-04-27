using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Experiment
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
            var bm = new Benchmark<SiblingLinkWeightedCombined, CutTreeInMidle, MojoFM>();
            var results = bm.Run(@"C:\Users\Galdiuz\git\parsed-csharp-repos\SignalR\Microsoft.AspNet.SignalR.Tests.Common");
            ;
        }
    }

}
