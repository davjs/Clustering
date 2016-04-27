using System;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics.MojoFM;
using Clustering.SolutionModel.Serializing;
using FluentAssertions;
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

            var cutTreeA = CutTreeInMidle.CutTreeAtDepth(treeA.Nodes, 2);
            var cutTreeB = CutTreeInMidle.CutTreeAtDepth(treeB.Nodes, 2);

            var result = new MojoFM().Calc(cutTreeA, cutTreeB);

            result.Should().Be(40);
        }
    }
}
