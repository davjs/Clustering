using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Clustering.Benchmarking.Results;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;
using MoreLinq;

namespace Clustering.Benchmarking
{
    public static class BenchMark
    {
        public static BenchMarkResult Run(IBenchmarkConfig config,NonNestedClusterGraph leafNamespacesWithDependencies)
        {
            var leafNamespaces = leafNamespacesWithDependencies.Clusters;
            if (leafNamespaces.Count < 2)
                return new BenchMarkResult("NOT_ENOUGH_LEAF_NAMESPACES");

            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToSet();

            var clusteredResults = config.ClusteringAlgorithm.Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutClusters = config.CuttingAlgorithm.Cut(clusteredResults).ToSet();

            var accuracy = config.SimilarityMectric.Calc(cutClusters, leafNamespaces);
            return new BenchMarkResult(accuracy);
        }

        public static BenchMarkResult CompareResultsOf2Algs(
            ClusteringAlgorithm alg1, ClusteringAlgorithm alg2,
            ICuttingAlgorithm cuttingAlgorithm,
            SimilarityMetrics.ISimilarityMectric metric, 
            NonNestedClusterGraph leafNamespacesWithDependencies)
        {
            var leafNamespaces = leafNamespacesWithDependencies.Clusters;
            if (leafNamespaces.Count < 2)
                return new BenchMarkResult("NOT_ENOUGH_LEAF_NAMESPACES");

            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToSet();

            var clusteredResults1 = alg1.Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var clusteredResults2 = alg2.Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutResults1 = cuttingAlgorithm.Cut(clusteredResults1);
            var cutResults2 = cuttingAlgorithm.Cut(clusteredResults2);

            var accuracy = metric.Calc(cutResults1, cutResults2);
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
            var leafNodes = unnestedRoots.SelectMany(x => x.Children);

            // Skip all keys not in leafNodes
            var leafNodeDeps = treeWithDependencies
                .Edges.Where(x => leafNodes.Contains(x.Key));
            
            var newDependencyLookUp = leafNodeDeps.SelectMany(group => @group,
                (group, node) => new { @group.Key, node })
            // Skip all values not in leafNodes
                .Where(x => leafNodes.Contains(x.node))
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
            // Skip all values not in leafNodes
                .Where(x => leafNodes.Contains(x.node))
                .ToLookup(x => x.Key,x => x.node);

            return new NonNestedClusterGraph(leafNamespaces, newDependencyLookUp);
        }
    }
}