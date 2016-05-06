using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;

namespace Clustering.Benchmarking
{
    public static class BenchMark
    {
        private static BenchMarkResult Run(IBenchmarkConfig config,NonNestedClusterGraph leafNamespacesWithDependencies)
        {
            var leafNamespaces = leafNamespacesWithDependencies.Clusters;
            if (leafNamespaces.Count < 2)
                return new BenchMarkResult("NOT_ENOUGH_LEAF_NAMESPACES");

            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();

            var clusteredResults = config.ClusteringAlgorithm.Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutClusters = config.CuttingAlgorithm.Cut(clusteredResults).ToImmutableHashSet();

#if DEBUG
            var leafnodes1 = cutClusters.SelectMany(x => x.Children).ToImmutableHashSet();
            var leafnodes2 = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();
            Debug.Assert(leafnodes1.SetEquals(leafnodes2));
#endif

            var accuracy = config.SimilarityMectric.Calc(cutClusters, leafNamespaces);
            return new BenchMarkResult(accuracy);
        }

        public static IEnumerable<BenchMarkResultsEntry> RunAllInFolder(IBenchmarkConfig config,IEnumerable<ProjectTreeWithDependencies> folderName)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in folderName)
            {
                var leafNamespacesWithDependencies = GetLeafNamespaces(file);
                yield return new BenchMarkResultsEntry(file.Name,Run(config,leafNamespacesWithDependencies));
            }
        }

        public static void Prepare(string solution,string outputDir)
        {
            var solutionModel = SolutionModelBuilder.FromPath(solution);
            SolutionModelRasterizer.Write(solutionModel, outputDir);
        }
        public static IEnumerable<ProjectTreeWithDependencies> GetProjectGraphsInFolder(string folderName)
        {
            var files = Directory.GetFiles(folderName);
            var notTests = files.Where(x => !x.ToLower().Contains("test")
                                            && x.ToLower().EndsWith(".flat"));
            return from file in notTests
                let fileContents = File.ReadAllText(file)
                let projectName = Path.GetFileNameWithoutExtension(file)
                select new ProjectTreeWithDependencies(projectName, GraphDecoder.Decode(fileContents));
        }

        public static NonNestedClusterGraph GetLeafNamespaces(ProjectTreeWithDependencies treeWithDeps)
        {
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters().ToImmutableHashSet();
            var leafNodes = leafNamespaces.SelectMany(x => x.Children);
            var leafNodeDeps = treeWithDeps.Edges.Where(x => leafNodes.Contains(x.Key));

            var newDependencyLookUp = leafNodeDeps.SelectMany(group => @group, 
                (group, node) => new {@group.Key,node})
                .ToLookup(x => x.Key,x => x.node);

            return new NonNestedClusterGraph(leafNamespaces, newDependencyLookUp);
        }
    }
}