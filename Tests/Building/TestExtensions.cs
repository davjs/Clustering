using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;

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

    public static class ClusterTestSetup
    {
        public static ProjectTreeWithDependencies Setup(string nodeCreationQuery)
        {
            nodeCreationQuery = nodeCreationQuery.Trim();
            nodeCreationQuery = Regex.Replace(nodeCreationQuery, @"[ \r]", "");
            var entries = nodeCreationQuery.Split('\n').ToList();
            var splitEntries =
                entries.Select(x => x.Split(new[] {"->"}, StringSplitOptions.RemoveEmptyEntries)).ToList();

            var nodes = splitEntries.Select(x => new NamedNode(x[0])).ToList();
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

            return new ProjectTreeWithDependencies
                ( nodes.Cast<Node>().ToSet()
                , totalDependencies.ToLookup(x => x.Dependor, x => x.Dependency));
        }
    }

    internal class NodeNotFoundInListException : Exception
    {
        public NodeNotFoundInListException(string dependency) : base(dependency)
        {
        }
    }
}