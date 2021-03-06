﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Clustering.Hierarchichal.DirectLinks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public abstract class ClusteringAlgorithm
    {
        protected HashSet<Node> Nodes;
        private SimilarityMatrix _matrix;

        protected abstract void Setup(ISet<Node> nodes, ILookup<Node, Node> edges);

        private void _Setup(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            Nodes = nodes.ToMutableSet();
            Setup(nodes,edges);
        }

        public ISet<Node> Cluster(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _Setup(nodes, edges);
            _matrix = CreateSimilarityMatrix(Nodes);

            while (Nodes.Count > 2)
            {
                var closest = _matrix.GetMostSimilar();
                var pair = closest.Key;
                var similarity = closest.Value;
                CreateCluster(pair.Item1, pair.Item2,similarity);
            }
            return Nodes;
        }

        private void CreateCluster(Node item1, Node item2, double similarity)
        {
            Nodes.Remove(item1);
            Nodes.Remove(item2);
            var clusterNode = new ClusterNode(similarity,item1, item2);
            UpdateSimilarityMatrix(item1,item2,clusterNode,_matrix);
            Nodes.Add(clusterNode);
        }

        protected abstract void UpdateSimilarityMatrix(Node item1, Node item2,
            ClusterNode clusterNode, SimilarityMatrix matrix);

        protected abstract double Similarity(Node left, Node right);

        private SimilarityMatrix CreateSimilarityMatrix(ISet<Node> nodes)
        {
            var simMatrix = new SimilarityMatrix();
            var nodesList = nodes.ToList();
            var pairs = nodesList.SelectMany((value, index) => nodesList.Skip(index + 1),
                               (left, right) => new { left, right });

            foreach (var pair in pairs)
                simMatrix.Add(pair.left, pair.right, Similarity(pair.left, pair.right));

            return simMatrix;
        }

        protected static double Ellenberg(FeatureVector left, FeatureVector right)
        {
            var both = left.Intersect(right);
            var onlyLeft = left.Except(right);
            var onlyRight = right.Except(left);

            var maHalf = both.Sum(x => left[x]/left.Total
                                       + right[x]/right.Total)*0.5;

            var mb = onlyLeft.Sum(x => left[x])/left.Total;
            var mc = onlyRight.Sum(x => right[x])/right.Total;

            if (maHalf + mb + mc <= 0) return 0;
            // MaHalf + Mb + Mc > 0
            return maHalf/(maHalf + mb + mc);
        }


        protected static double UnbiasedEllenberg(FeatureVector left, FeatureVector right)
        {
            var both = left.Intersect(right);
            var onlyLeft = left.Except(right);
            var onlyRight = right.Except(left);

            var maHalf = both.Sum(x => left[x] / left.Total
                                       + right[x] / right.Total) * 0.5;

            var mb = onlyLeft.Count;//onlyLeft.Sum(x => left[x])/left.Total;
            var mc = onlyRight.Count;//onlyRight.Sum(x => right[x])/right.Total;

            if (maHalf + mb + mc <= 0) return 0;
            // MaHalf + Mb + Mc > 0
            return maHalf / (maHalf + mb + mc);
        }
    }
}