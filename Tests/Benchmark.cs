using System.Collections.Generic;
using System.IO;
using Clustering.Cutting;
using Clustering.Hierarchichal;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;

namespace Tests
{
    class Benchmark<TClusterAlg,TCuttingAlg,TMetric> 
        where TClusterAlg : ClusteringAlgorithm, new()
        where TCuttingAlg : ICuttingAlgorithm, new()
        where TMetric : IMectric, new ()                                     
    {
        public BenchmarkResults Run(string inputFIle)
        {
            var content = File.ReadAllText(inputFIle);
            var treeWithDeps = GraphDecoder.Decode(content);
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters();

            var clusteredResults = new TClusterAlg().Cluster(treeWithDeps.Nodes,treeWithDeps.Edges);
            var cutClusters = new TCuttingAlg().Cut(clusteredResults);
            var accuracy = new TMetric().ClusterSimilarity(leafNamespaces,cutClusters);

            return new BenchmarkResults(accuracy);
        }
    }

    internal interface IMectric
    {
        double ClusterSimilarity(IEnumerable<Node> created, IEnumerable<Node> groundTruth);
    }

    class BenchMark
    {
        public static void Prepare(string solutionDir,string outputDir)
        {
            
        }
    }

    class BenchmarkResults
    {
        public readonly double Accuracy;

        public BenchmarkResults(double accuracy)
        {
            Accuracy = accuracy;
        }
    }
}