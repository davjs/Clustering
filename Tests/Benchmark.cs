using System.Collections.Generic;
using System.Diagnostics;
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
            var leafNodes = treeWithDeps.Nodes.SelectMany(x => x.LeafNodes()).ToSet();


            var admin = leafNodes.WithName("AdminAuthHub");
            var deps = treeWithDeps.Edges[admin];
            var admin2 = treeWithDeps.Edges.ToList()[0].Key;
            Debug.Assert(admin == admin2);

            var clusteredResults = new TClusterAlg().Cluster(leafNodes, treeWithDeps.Edges);
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