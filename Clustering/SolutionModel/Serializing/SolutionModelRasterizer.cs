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
            var projectNodes = solutionModel.Projects().ToList();
            foreach (var projectNode in projectNodes)
            {
                var dependencies = DependencyResolver.GetDependencies(projectNode.Classes());
                EncodeTreeToFlat(path, projectNode.Children, dependencies,projectNode.Name);
            }

            // Write complete solution file
            var allClasses = projectNodes.SelectMany(x => x.Classes());
            var dependenciesAll = DependencyResolver.GetDependencies(allClasses);
            EncodeTreeToFlat(path, projectNodes, dependenciesAll, "complete");

        }

        private static void EncodeTreeToFlat(string path, IEnumerable<Node> nodes, ILookup<Node, Node> dependencies,string fname)
        {
            var encodedString = Encode.HierarchicalGraph(nodes,
                n => n.Children,
                n => n.Name,
                n => dependencies[n]);
            var sha1 = SHA1Util.SHA1HashStringForUTF8String(encodedString);
            var finalString = sha1 + "\n" + encodedString;
            Directory.CreateDirectory(path);
            File.WriteAllText(path + fname + ".flat", finalString);
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
        public readonly ISet<Node> Nodes;
        public readonly ILookup<Node, Node> Edges;
        public ProjectTreeWithDependencies(string name,TreeWithDependencies<Node> treeWithDependencies)
        {
            Name = name;
            Nodes = treeWithDependencies.Tree.ToSet();
            Edges = treeWithDependencies.Dependencies;
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
