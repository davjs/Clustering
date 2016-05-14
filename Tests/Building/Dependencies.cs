using System;
using System.Diagnostics;
using System.Linq;
using Clustering.Benchmarking;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;
using Flat;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Building
{
    [TestClass]
    public class Dependencies
    {
        [TestMethod]
        public void ResolvesDependencies()
        {
            var solutionModel = SolutionModelBuilder.FromPath(TestExtensions.Paths.ThisSolution);
            var allClasses = solutionModel.Projects().SelectMany(x => x.Classes()).ToList();
            var dependencies = DependencyResolver.GetDependencyList(allClasses).ToList();

            //Assert
            dependencies.Should().Contain(x => x.Dependor.Name == "Dependencies"
                                               && x.Dependency.Name == "SolutionModelBuilder");
        }

        [TestMethod]
        public void FlatClusterGraphOnlyContainsInternalDependencies()
        {
            var solutionModel = SolutionModelBuilder.FromPath(TestExtensions.Paths.ThisSolution);

            var allClasses = solutionModel.Projects()
                .SelectMany(p => p.Classes()).ToList();

            var deps = DependencyResolver.GetDependencies(allClasses);

            var pTree = new ProjectTreeWithDependencies("x",new TreeWithDependencies<Node>(solutionModel.Children, deps));

            deps.Should().NotContain(depGroup => !allClasses.Contains(depGroup.Key) ||
                                                 depGroup.Except(allClasses).Any());

            var rootNamespaces = BenchMark.RootNamespaces(pTree);

            var leafNodes = rootNamespaces.Clusters.SelectMany(x => x.Children).ToList();
            
            rootNamespaces.Edges.Should().NotContain(depGroup =>
                !leafNodes.Contains(depGroup.Key)
                || depGroup.Except(leafNodes).Any());
        }
    }
}
