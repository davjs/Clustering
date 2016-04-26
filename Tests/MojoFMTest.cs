using System;
using Clustering.Cutting;
using Clustering.MojoFM;
using Clustering.SolutionModel.Serializing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MojoFMTest
    {
        [TestMethod]
        public void MojoTest()
        {
            var treeA = GraphDecoder.Decode(@"
                @A:
                @A\B:
                @A\B\BA:
                @A\B\BA\BAA:
                @A\B\BA\BAB:
                @A\B\BB:
                @A\B\BB\BBA:
                @A\B\BB\BBB:
                @A\C:
                @A\C\CA:
                @A\C\CA\CAA:
                @A\C\CA\CAB:
                @A\C\CB:
                @A\C\CB\CBA:
                @A\C\CB\CBB:
            ");

            var treeB = GraphDecoder.Decode(@"
                @A:
                @A\B:
                @A\B\BA:
                @A\B\BA\BAA:
                @A\B\BA\BAB:
                @A\B\BA\BBA:
                @A\B\BB:
                @A\B\BB\BBB:
                @A\C:
                @A\C\CA:
                @A\C\CA\CAA:
                @A\C\CA\CBA:
                @A\C\CB:
                @A\C\CB\CAB:
                @A\C\CB\CBB:
            ");

            var cutTreeA = CuttingAlgorithm.GetNodes(treeA.Nodes, 2);
            var cutTreeB = CuttingAlgorithm.GetNodes(treeB.Nodes, 2);

            var result = MojoFM.Calc(cutTreeA, cutTreeA);
            ;
        }
    }
}
