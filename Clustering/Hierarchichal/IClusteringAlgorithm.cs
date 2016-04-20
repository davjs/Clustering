using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public abstract class ClusteringAlgorithm
    {
        protected HashSet<Node> _nodes;
        private SimilarityMatrix _matrix;

        protected abstract void Setup(ISet<Node> nodes, ILookup<Node, Node> edges);
        protected abstract SimilarityMatrix CreateSimilarityMatrix(ISet<Node> nodes);

        private void _Setup(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _nodes = nodes.ToMutableSet();
            Setup(nodes,edges);
        }

        public ISet<Node> Cluster(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _Setup(nodes, edges);
            _matrix = CreateSimilarityMatrix(_nodes);

            while (_nodes.Count > 2)
            {
                var closest = _matrix.GetMostSimilar();
                var pair = closest.Key;
                var similarity = closest.Value;
                CreateCluster(pair.Item1, pair.Item2,similarity);
            }
            return _nodes;
        }

        private void CreateCluster(Node item1, Node item2, double similarity)
        {
            _nodes.Remove(item1);
            _nodes.Remove(item2);
            var clusterNode = new ClusterNode(similarity,item1, item2);
            UpdateSimilarityMatrix(item1,item2,clusterNode,_matrix);
            _nodes.Add(clusterNode);
        }

        protected abstract void UpdateSimilarityMatrix(Node item1, Node item2,
            ClusterNode clusterNode, SimilarityMatrix matrix);
        
        public abstract double Similarity(FeatureVector a, FeatureVector b);
    }
}