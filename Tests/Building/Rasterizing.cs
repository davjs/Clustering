using System;
using System.IO;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Building
{
    [TestClass]
    public class Rasterizing
    {
        [TestMethod]
        public void Write()
        {
            var solutionModel = SolutionModelBuilder.FromPath(TestExtensions.SolutionPaths.ThisSolution);
            Resterizer.Write(solutionModel);
        }
    }

    public class Resterizer
    {
        public static void Write(SolutionNode solutionModel)
        {
            foreach (var projectNode in solutionModel.Projects())
            {
                var dependencies = DependencyResolver.GetDependencies(projectNode.Classes());
                var encodedString = projectNode.Children.EncodeGraph(
                    n => n.Children,
                    n => n.Name, 
                    n => dependencies[n]);
                File.WriteAllText(projectNode.Name,encodedString);
            }
        }
    }
}
