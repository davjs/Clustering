using System.IO;
using Clustering.Hierarchichal;
using Clustering.SolutionModel.Serializing;

namespace Tests
{
    class Benchmark<T> where T : ClusteringAlgorithm, new()
    {
        public BenchmarkResults Run(string inputFIle)
        {
            var content = File.ReadAllText(inputFIle);
            var model = GraphDecoder.Decode(content);
            var clusteredResults = new T().Cluster(model.Nodes,model.Edges);
            ////


            ////
            return new BenchmarkResults();
        }
    }

    class BenchmarkResults
    {

    }
}