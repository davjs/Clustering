using System.Collections.Generic;
using System.IO;
using System.Linq;
using Clustering.Hierarchichal;
using Clustering.Hierarchichal.CuttingAlgorithms;
using Clustering.SimilarityMetrics;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Clustering.SolutionModel.Serializing;

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
            var leafNamespaces = treeWithDeps.Nodes.LeafClusters();

            var clusteredResults = new TClusterAlg().Cluster(treeWithDeps.Nodes,treeWithDeps.Edges);
            var cutClusters = new TCuttingAlg().Cut(clusteredResults);
            return new TMetric().Calc(leafNamespaces,cutClusters);
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