using System;
using System.Collections.Generic;
using System.Linq;
using Clustering.Hierarchichal;
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

            var leafNodes = treeA.Tree.SelectMany(x => x.Children);

            var treeBNodes = leafNodes.Reverse().ChunkBy(2)
                .Select(x => new ClusterNode().WithChildren(x));

            var result = new MojoFM().Calc(treeA.Tree, treeBNodes);

            result.Should().Be(100);
        }
    }

    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }
    }
}
