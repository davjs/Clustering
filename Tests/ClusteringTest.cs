using Clustering.Hierarchichal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building.TestExtensions;

namespace Tests
{
    [TestClass]
    public class ClusteringTest
    {
        [TestMethod]
        public void CreatesPairForSimilarFeature()
        {
            var model = ClusterTestSetup.Setup(
                @"
                C ->
                A -> C
                B -> C");

            // Act
            var newTree = new SiblingLinkWeightedCombinedDepOnly()
                .Cluster(model.Nodes,model.Edges);

            // Assert
            newTree.Should().Contain(x => x.Name == "C");
            newTree.Should().Contain(x => x is ClusterNode)
                .Which.Children.Should()
                .Contain(x => x.Name == "A")
                .And.Contain(x => x.Name == "B");
        }
    }
}
