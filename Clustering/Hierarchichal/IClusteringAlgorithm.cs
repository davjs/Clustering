﻿using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public abstract class ClusteringAlgorithm
    {
        protected readonly HashSet<Node> _nodes;
        private SimilarityMatrix _matrix;
        public abstract SimilarityMatrix CreateSimilarityMatrix(ISet<Node> nodes);

        public ClusteringAlgorithm(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _nodes = nodes.ToMutableSet();
        }

        public ISet<Node> Cluster()
        {
            _matrix = CreateSimilarityMatrix(_nodes);

            while (_nodes.Count > 2)
            {
                var closest = _matrix.GetMostSimilar();
                CreateCluster(closest.Item1, closest.Item2);
            }
            return _nodes;
        }

        private void CreateCluster(Node item1, Node item2)
        {
            _nodes.Remove(item1);
            _nodes.Remove(item2);
            var clusterNode = new ClusterNode(item1, item2);
            UpdateSimilarityMatrix(item1,item2,clusterNode,_matrix);
            _nodes.Add(clusterNode);
        }

        protected abstract void UpdateSimilarityMatrix(Node item1, Node item2,
            ClusterNode clusterNode, SimilarityMatrix matrix);

        //public abstract void UpdateSimilarityMatrix();

        public abstract double Similarity(FeatureVector a, FeatureVector b);
    }
}