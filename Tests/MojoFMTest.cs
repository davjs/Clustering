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
                @BA:
                @BA\BAA:
                @BA\BAB:
                @BB:
                @BB\BBA:
                @BB\BBB:
                @CA:
                @CA\CAA:
                @CA\CAB:
                @CB:
                @CB\CBA:
                @CB\CBB:
            ");

            var treeB = GraphDecoder.Decode(@"
                @BA:
                @BA\BAA:
                @BA\BAB:
                @BA\BBA:
                @BB:
                @BB\BBB:
                @CA:
                @CA\CAA:
                @CA\CBA:
                @CB:
                @CB\CAB:
                @CB\CBB:
            ");

            var result = new MojoFM().Calc(treeA.Nodes, treeB.Nodes);

            result.Should().Be(40);
        }
    }
}
