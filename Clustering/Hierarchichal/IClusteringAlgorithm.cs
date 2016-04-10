using System.Collections.Generic;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public abstract class ClusteringAlgorithm
    {
        private readonly ISet<Node> _nodes;
        private readonly ISet<DependencyLink> _edges;
        public abstract ISimilarityMatrix CreateSimilarityMatrix(ISet<Node> nodes, ISet<DependencyLink> edges);

        ClusteringAlgorithm(ISet<Node> nodes, ISet<DependencyLink> edges)
        {
            _nodes = nodes;
            _edges = edges;
        }

        public ISet<Node> Cluster()
        {
            var matrix = CreateSimilarityMatrix(_nodes, _edges);

            while (_nodes.Count > 2)
            {
                var closest = matrix.GetMostSimilar();
                CreateCluster(closest.Item1, closest.Item2);
            }
            return _nodes;
        }

        private void CreateCluster(Node item1, Node item2)
        {
            _nodes.Remove(item1);
            _nodes.Remove(item2);

            var clusterNode = new ClusterNode(item1, item2);
            _nodes.Add(clusterNode);
        }

        public abstract double Similarity(FeatureVector a, FeatureVector b);
    }
}