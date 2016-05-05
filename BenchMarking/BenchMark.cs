using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;

namespace BenchMarking
{
    public class Benchmark<TClusterAlg, TCuttingAlg, TMetric>
        where TClusterAlg : ClusteringAlgorithm, new()
        where TCuttingAlg : ICuttingAlgorithm, new()
        where TMetric : ISimilarityMectric, new()
    {
        private double Run(ProjectTreeWithDependencies treeWithDeps)
        {
            var leafNamespacesWithDependencies = GetLeafNamespaces(treeWithDeps);
            var leafNamespaces = leafNamespacesWithDependencies.Nodes;
            if (leafNamespaces.Count < 2)
                throw new NotEnoughLeafNamespacesException();
            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToSet();

            var clusteredResults = new TClusterAlg().Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutClusters = new TCuttingAlg().Cut(clusteredResults).ToSet();

#if DEBUG
            var leafnodes1 = cutClusters.SelectMany(x => x.Children).ToSet();
            var leafnodes2 = leafNamespaces.SelectMany(x => x.Children).ToSet();
            Debug.Assert(leafnodes1.SetEquals(leafnodes2));
#endif

            return new TMetric().Calc(cutClusters, leafNamespaces);
        }

        public IEnumerable<BenchMarkResult> RunAllInFolder(string folderName)
        {
            var files = Directory.GetFiles(folderName);
            var notTests = files.Where(x => !x.ToLower().Contains("test")
                                            && x.ToLower().EndsWith(".flat"));
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in notTests)
            {
                var projectName = Path.GetFileNameWithoutExtension(file);
                var projectGraph = GraphDecoder.Decode(File.ReadAllText(file));
                BenchMarkResult benchMarkResult;
                try
                {
                    var accuracy = Run(projectGraph);
                    benchMarkResult = new BenchMarkResult(projectName, accuracy);
                }
                    // Skip if less than 2 leaf namespaces
                catch (NotEnoughLeafNamespacesException e)
                {
                    benchMarkResult = new BenchMarkResult(projectName, e);
                }
                yield return benchMarkResult;
            }
        }

        private ProjectTreeWithDependencies GetLeafNamespaces(ProjectTreeWithDependencies treeWithDeps)
        {
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters().ToSet();
            var leafNodes = leafNamespaces.SelectMany(x => x.Children);
            var leafNodeDeps = treeWithDeps.Edges.Where(x => leafNodes.Contains(x.Key));

            var newDependencyLookUp = leafNodeDeps.SelectMany(group => group,
                (group, node) => new { group.Key, node })
                .ToLookup(x => x.Key, x => x.node);

            return new ProjectTreeWithDependencies(leafNamespaces, newDependencyLookUp);
        }
    }

    public static class BenchMark
    {
        public static void Prepare(string solution, string outputDir)
        {
            var solutionModel = SolutionModelBuilder.FromPath(solution);
            if(Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);
            SolutionModelRasterizer.Write(solutionModel, outputDir);
        }
    }
}