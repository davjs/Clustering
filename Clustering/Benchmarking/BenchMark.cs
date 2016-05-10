using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;
using MoreLinq;

namespace Clustering.Benchmarking
{
    public static class BenchMark
    {
        public static BenchMarkResult Run(IBenchmarkConfig config,NonNestedClusterGraph leafNamespacesWithDependencies)
        {
            var configCopy = config.Clone();
            var leafNamespaces = leafNamespacesWithDependencies.Clusters;
            //if (leafNamespaces.Count < 2)
            //    return new BenchMarkResult("NOT_ENOUGH_LEAF_NAMESPACES");

            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();

            var clusteredResults = configCopy.ClusteringAlgorithm.Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutClusters = configCopy.CuttingAlgorithm.Cut(clusteredResults).ToImmutableHashSet();

#if DEBUG
            var leafnodes1 = cutClusters.SelectMany(x => x.Children).ToImmutableHashSet();
            var leafnodes2 = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();
            Debug.Assert(leafnodes1.SetEquals(leafnodes2));
#endif

            var accuracy = configCopy.SimilarityMectric.Calc(cutClusters, leafNamespaces);
            return new BenchMarkResult(accuracy);
        }

        public static void Prepare(string solution,string outputDir)
        {
            var solutionModel = SolutionModelBuilder.FromPath(solution);
            SolutionModelRasterizer.Write(solutionModel, outputDir);
        }


        public static ProjectTreeWithDependencies GetCompleteTreeWithDependencies(string folderName)
        {
            var files = Directory.GetFiles(folderName);
            var complete = files.First(x => Path.GetFileNameWithoutExtension(x) == "complete");
            
            var fileContents = File.ReadAllText(complete);
            var projectName = Path.GetFileNameWithoutExtension(complete);
            return new ProjectTreeWithDependencies(projectName, GraphDecoder.Decode(fileContents));
        }

        public static IEnumerable<ProjectTreeWithDependencies> GetProjectGraphsInFolder(string folderName)
        {
            var files = Directory.GetFiles(folderName);
            var notTests = files.Where(x => !x.ToLower().Contains("test")
                                            && x.ToLower().EndsWith(".flat"));

            var notComplete = notTests.Where(x => Path.GetFileNameWithoutExtension(x) == "complete");

            return from file in notComplete
                   let fileContents = File.ReadAllText(file)
                let projectName = Path.GetFileNameWithoutExtension(file)
                select new ProjectTreeWithDependencies(projectName, GraphDecoder.Decode(fileContents));
        }

        public static NonNestedClusterGraph RootNamespaces(ProjectTreeWithDependencies treeWithDependencies)
        {
            var roots = treeWithDependencies.Nodes.Where(x => x.Children.Any() && !x.Name.ToLower().Contains("test"));
            var unnestedRoots = roots.Select(x => x.WithChildren(x.LeafNodes())).ToHashSet();
            var leafNodeDeps = treeWithDependencies
                .Edges.Where(x => unnestedRoots.Any(r => r.Children.Contains(x.Key)));


            var newDependencyLookUp = leafNodeDeps.SelectMany(group => @group,
                (group, node) => new { @group.Key, node })
                .ToLookup(x => x.Key, x => x.node);

            return new NonNestedClusterGraph(unnestedRoots, newDependencyLookUp);
        }
        
        public static NonNestedClusterGraph GetLeafNamespaces(ProjectTreeWithDependencies treeWithDeps)
        {
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters().ToHashSet();
            var leafNodes = leafNamespaces.SelectMany(x => x.Children);
            var leafNodeDeps = treeWithDeps.Edges.Where(x => leafNodes.Contains(x.Key));

            var newDependencyLookUp = leafNodeDeps.SelectMany(group => @group, 
                (group, node) => new {@group.Key,node})
                .ToLookup(x => x.Key,x => x.node);

            return new NonNestedClusterGraph(leafNamespaces, newDependencyLookUp);
        }
    }
}