using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Tests.Building
{
    public static class TestExtensions
    {
        public static class SolutionPaths
        {
            public static readonly string ThisSolution = AppDomain.CurrentDomain.BaseDirectory +
                                                         "\\..\\..\\..\\Clustering.sln";
        }
    }

    public class FakeNode : Node
    {
        public FakeNode(string name, IEnumerable<Node> children = null, Node parent = null)
            : base(name, children, parent)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children) =>
            new FakeNode(Name, children, Parent);

        public override Node WithParent(Node parent) =>
            new FakeNode(Name, Children, parent);
    }

    public static class ClusterTestSetup
    {
        public class TestModel
        {
            public ISet<Node> Nodes;
            public ILookup<Node,Node> Dependencies;
        }

        public static TestModel Setup(string nodeCreationQuery)
        {
            nodeCreationQuery = nodeCreationQuery.Trim();
            nodeCreationQuery = Regex.Replace(nodeCreationQuery, @"[ \r]", "");
            var entries = nodeCreationQuery.Split('\n').ToList();
            var splitEntries =
                entries.Select(x => x.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries)).ToList();

            var nodes = splitEntries.Select(x => new FakeNode(x[0])).ToList();
            var totalDependencies = new HashSet<DependencyLink>();

            var i = 0;
            foreach (var dependencyString in splitEntries.Select(x => x.Length == 2 ? x[1] : null))
            {
                if (dependencyString != null)
                {
                    var dependencies = dependencyString.Split(',');
                    foreach (var dependency in dependencies)
                    {
                        var dependencyNode = nodes.WithName(dependency);
                        if (dependencyNode == nodes[i])
                            throw new Exception("Can not be dependant on itself");
                        if (dependencyNode == null)
                            throw new NodeNotFoundInListException(dependency);
                    }
                    // Get the actual nodes of the dependencies
                    var nodeDependencies = dependencies.Select(x => nodes.WithName(x));
                    totalDependencies.UnionWith(nodeDependencies.Select(x => new DependencyLink(nodes[i], x)));
                }
                i++;
            }

            return new TestModel
            { Dependencies = totalDependencies.ToLookup(x => x.Dependor,x => x.Dependency),
              Nodes = nodes.Cast<Node>().ToSet() };
        }
    }

    internal class NodeNotFoundInListException : Exception
    {
        public NodeNotFoundInListException(string dependency) : base(dependency)
        {
        }
    }
}