using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;
using Flat;

namespace Clustering.SolutionModel.Serializing
{
    public static class SolutionModelRasterizer
    {
        public static void Write(SolutionNode solutionModel, string path)
        {
            foreach (var projectNode in solutionModel.Projects())
            {
                var dependencies = DependencyResolver.GetDependencies(projectNode.Classes());
                var encodedString = Encode.HierarchicalGraph(projectNode.Children,
                    n => n.Children,
                    n => n.Name,
                    n => dependencies[n]);
                var sha1 = SHA1Util.SHA1HashStringForUTF8String(encodedString);
                var finalString = sha1 + "\n" + encodedString;
                File.WriteAllText(path + projectNode.Name, finalString);
            }
        }
    }

    public class ProjectTreeWithDependencies
    {
        private readonly TreeWithDependencies<Node> _treeWithDependencies;
        public ISet<Node> Nodes => _treeWithDependencies.Tree;
        public ILookup<Node, Node> Edges => _treeWithDependencies.Dependencies;
        public ProjectTreeWithDependencies(TreeWithDependencies<Node> treeWithDependencies)
        {
            _treeWithDependencies = treeWithDependencies;
        }
    }

    public static class GraphDecoder
    {
        [SuppressMessage("ReSharper", "RedundantArgumentName", Justification = "Explicitness")]
        public static ProjectTreeWithDependencies Decode(string text)
        {
            return
                new ProjectTreeWithDependencies(
                    Flat.Decode.HierarchicalGraph<Node>(text,
                createNodeWithName: name => new NamedNode(name),
                addChildrenToNode: (previous, childrenToAdd) => previous.WithChildren(childrenToAdd)));
        }
    }
}
