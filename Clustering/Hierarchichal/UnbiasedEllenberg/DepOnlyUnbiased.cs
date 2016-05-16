using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Clustering.Hierarchichal.DirectLinks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal
{

    public class DepOnlyUnbiased : ClusteringAlgorithm
    {
        private Dictionary<Node, FeatureVector> _featureVectors;

        protected override void Setup(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _featureVectors = nodes.ToDictionary(
                dependor => dependor,
                dependor => new FeatureVector(edges[dependor].ToSet()));
        }
        
        protected override void UpdateSimilarityMatrix(Node item1, Node item2, ClusterNode clusterNode,
            SimilarityMatrix matrix)
        {
            _featureVectors.Add(clusterNode, _featureVectors[item1]
                .Merge(_featureVectors[item2]));

            foreach (var node in Nodes.Where(node => node != item1 && node != item2))
            {
                matrix.Add(node, clusterNode, Similarity(node,clusterNode));
                matrix.Remove(node, item1);
                matrix.Remove(node, item2);
            }

            matrix.Remove(item1, item2);

            _featureVectors.Remove(item1);
            _featureVectors.Remove(item2);
        }

        protected override double Similarity(Node left, Node right) =>
            UnbiasedEllenberg(_featureVectors[left], _featureVectors[right]);
    }
}