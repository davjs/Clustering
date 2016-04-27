using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;
using Flat;
using FluentAssertions;

namespace Tests
{
    class Benchmark<TClusterAlg,TCuttingAlg,TMetric> 
        where TClusterAlg : ClusteringAlgorithm, new()
        where TCuttingAlg : ICuttingAlgorithm, new()
        where TMetric : ISimilarityMectric, new ()                                     
    {
        public double Run(string inputFIle)
        {
            var content = File.ReadAllText(inputFIle);
            var treeWithDeps = GraphDecoder.Decode(content);
            var leafNamespacesWithDependencies = GetLeafNamespaces(treeWithDeps);
            var leafNamespaces = leafNamespacesWithDependencies.Nodes;

            var leafNodes = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();

            var clusteredResults = new TClusterAlg().Cluster(leafNodes, leafNamespacesWithDependencies.Edges);
            var cutClusters = new TCuttingAlg().Cut(clusteredResults).ToImmutableHashSet();

#if DEBUG
            var leafnodes1 = cutClusters.SelectMany(x => x.Children).ToImmutableHashSet();
            var leafnodes2 = leafNamespaces.SelectMany(x => x.Children).ToImmutableHashSet();
            Debug.Assert(leafnodes1.SetEquals(leafnodes2));
#endif

            return new TMetric().Calc(cutClusters, leafNamespaces);
        }

        private ProjectTreeWithDependencies GetLeafNamespaces(ProjectTreeWithDependencies treeWithDeps)
        {
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters().ToImmutableHashSet();
            var leafNodes = leafNamespaces.SelectMany(x => x.Children);
            var leafNodeDeps = treeWithDeps.Edges.Where(x => leafNodes.Contains(x.Key));

            var newDependencyLookUp = leafNodeDeps.SelectMany(group => group, 
                (group, node) => new {group.Key,node})
                .ToLookup(x => x.Key,x => x.node);

            return new ProjectTreeWithDependencies(new TreeWithDependencies<Node>(leafNamespaces, newDependencyLookUp));
        }
    }

    class BenchMark
    {
        public static void Prepare(string solution,string outputDir)
        {
            var solutionModel = SolutionModelBuilder.FromPath(solution);
            SolutionModelRasterizer.Write(solutionModel, outputDir);
        }
    }
}