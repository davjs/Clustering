using System;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building;

namespace Tests
{
    [TestClass]
    public class CuttingTest
    {
        [TestMethod]
        public void CuttingTestMethod()
        {
            var tree = GraphDecoder.Decode(@"
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
            var cutTree = CutTreeInMidle.CutTreeAtDepth(tree.Tree, 1).ToSet();

            ;
        }
    }
}
