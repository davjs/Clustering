using System;
using System.Linq;
using Clustering.SolutionModel;
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
            var solutionModel = SolutionModelBuilder.FromPath(TestExtensions.SolutionPaths.ThisSolution);
            var allClasses = solutionModel.Projects().SelectMany(x => x.Classes()).ToList();
            var dependencies = DependencyResolver.GetDependencyList(allClasses).ToList();

            //Assert
            dependencies.Should().Contain(x => x.Dependor.Name == "Dependencies"
                                               && x.Dependency.Name == "SolutionModelBuilder");
        }
    }
}
