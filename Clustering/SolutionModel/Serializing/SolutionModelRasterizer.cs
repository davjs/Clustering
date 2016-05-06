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
                Directory.CreateDirectory(path);
                File.WriteAllText(path + projectNode.Name + ".flat", finalString);
            }
        }
    }

    public class NonNestedClusterGraph
    {
        public readonly ISet<Node> Clusters;
        public readonly ILookup<Node, Node> Edges;
        public NonNestedClusterGraph(ISet<Node> clusters, ILookup<Node, Node> edges)
        {
            Clusters = clusters;
            Edges = edges;
        }
    }

    public class ProjectTreeWithDependencies
    {
        public readonly string Name;
        private readonly TreeWithDependencies<Node> _treeWithDependencies;
        public ISet<Node> Nodes => _treeWithDependencies.Tree;
        public ILookup<Node, Node> Edges => _treeWithDependencies.Dependencies;
        public ProjectTreeWithDependencies(string name,TreeWithDependencies<Node> treeWithDependencies)
        {
            Name = name;
            _treeWithDependencies = treeWithDependencies;
        }
    }

    public static class GraphDecoder
    {
        [SuppressMessage("ReSharper", "RedundantArgumentName", Justification = "Explicitness")]
        public static TreeWithDependencies<Node> Decode(string text)
        {
            return
                    Flat.Decode.HierarchicalGraph<Node>(text,
                createNodeWithName: (name,path) => new PathedNode(name,path),
                addChildrenToNode: (previous, childrenToAdd) => previous.WithChildren(childrenToAdd));
        }
    }
}
