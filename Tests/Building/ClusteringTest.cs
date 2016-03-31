using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.Hierarchichal;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Building
{
    [TestClass]
    public class ClusteringTest
    {
        [TestMethod]
        public void TestClustering()
        {
            var solutionModel = SolutionModelBuilder.FromPath(TestExtensions.SolutionPaths.ThisSolution);
            var testNamespace = solutionModel.Projects().WithName("Tests")
                .Children.WithName("Tests")
                .Children.WithName("TestNamespace");

            var classesInNamespace = testNamespace.Children.Cast<ClassNode>().ToList();
            var dependencies = DependencyResolver.ResolveDependencies(classesInNamespace).ToList();
            var newTree = new SiblingLinkWeightedCombined().Cluster(classesInNamespace, dependencies);


            //Assert
            newTree.Should().Contain(x => x.Name == "C");
            newTree.Should().Contain(x => x is ClusterNode)
                .Which.Children.Should().Contain(x => x.Name == "A")
                .And.Contain(x => x.Name == "B");
        }
    }
}
