using System.Linq;
using Clustering.SolutionModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building.TestExtensions;

namespace Tests.Building
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void FindsClasses()
        {
            var solutionModel = SolutionModelBuilder.FromPath(Paths.ThisSolution);

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
        public void FindsParentNamespacesNotFoundAsSyntaxNodes()
        {
            using (var disposableResult = RoslynFaker.GetSymbolTreeFromDocuments(
                @"               
                    namespace A.B.C {class X{}}
                    namespace B.C {class X{}}
                    namespace C {class X{}}

                    namespace A.B.C.D {class X{}}
                "))
            {
                var tree = disposableResult.result.ToList();
                tree.Should().ContainSingle(x => x.Name == "C");
                tree.Should().ContainSingle(x => x.Name == "B");

                var A = tree.WithName("A");
                A.Should().NotBeNull();

                A.Children.Should().Contain(x => x.Name == "B")
                    .Which.Children.Should().Contain(x => x.Name == "C")
                        .Which.Children.Should().Contain(x => x.Name == "D");
            }
        }

        [TestMethod]
        public void FindsParentNamespacesNotFoundAsSyntaxNodes2()
        {
            using (var disposableResult = RoslynFaker.GetSymbolTreeFromDocuments(
                @"               
                    namespace A.B.C.D {class X{}}
                "))
            {
                var tree = disposableResult.result.ToList();
                
                tree.Should().Contain(x => x.Name == "A")
                    .Which.Children.Should().Contain(x => x.Name == "B")
                        .Which.Children.Should().Contain(x => x.Name == "C")
                            .Which.Children.Should().Contain(x => x.Name == "D");
            }
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
