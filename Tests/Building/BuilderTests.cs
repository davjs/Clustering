using System;
using System.Linq;
using Clustering.SolutionModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Building.TestExtensions;

namespace Tests
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void FindsClasses()
        {
            var solutionModel = SolutionModelBuilder.FromPath(SolutionPaths.ThisSolution);

            var projects = solutionModel.Projects().ToList();
            
            // Assert that we find ourselves
            projects
                .Should()
                .Contain(x => x.Name == "Tests")
                .Which.Classes()
                .Should()
                .Contain(x => x.Name == "BuilderTests");
        }

        [TestMethod]
        public void FilterTemplate()
        {
            // --- Same thing as above, just a reminder about unchaining from previous parent  ---
            // var classes = project.Classes();
            // Safety first, in case we want to use the parent variable in our algorithm later we remove the previous parent
            // var withoutParents = classes.Select(x => x.WithParent(null));
        }
    }
}
