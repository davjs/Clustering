﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Clustering.Hierarchichal.DirectLinks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal
{
    public class SiblingLinkWeightedCombinedSepUsage : ClusteringAlgorithm
    {
        private class FeatureVectorContainer
        {
            public readonly FeatureVector DependencyFeatureVectors;
            public readonly FeatureVector UsageFeatureVectors;

            public FeatureVectorContainer(FeatureVector dependencyFeatureVector, FeatureVector usageFeatureVector)
            {
                DependencyFeatureVectors = dependencyFeatureVector;
                UsageFeatureVectors = usageFeatureVector;
            }

            public FeatureVectorContainer Merge(FeatureVectorContainer other)
            {
                return new FeatureVectorContainer(
                    DependencyFeatureVectors.Merge(other.DependencyFeatureVectors),
                    UsageFeatureVectors.Merge(other.UsageFeatureVectors));
            }
        }

        private Dictionary<Node, FeatureVectorContainer> _featureVectors;

        protected override void Setup(ISet<Node> nodes, ILookup<Node, Node> dependencies)
        {
            var usages = nodes.ToDictionary(
                dependency => dependency,
                dependency => new List<Node>());

            foreach (var edge in dependencies)
            {
                var dependent = edge.Key;
                foreach (var dependency in edge)
                {
                    // TODO: Is this check really needed ?
                    //if (!usages.ContainsKey(dependency))
                    //    usages[dependency] = new List<Node>();
                    usages[dependency].Add(dependent);
                }
            }

            _featureVectors = nodes.ToDictionary(
                node => node,
                node => new FeatureVectorContainer(
                    dependencyFeatureVector: new FeatureVector(dependencies[node].ToSet()),
                    usageFeatureVector: new FeatureVector(usages[node].ToSet())));
        }

        protected override double Similarity(Node left, Node right) => 
            Similarity(_featureVectors[left], _featureVectors[right]);

        private double Similarity(FeatureVectorContainer left, FeatureVectorContainer right)
            => Ellenberg(left.DependencyFeatureVectors, right.DependencyFeatureVectors)
               + Ellenberg(left.UsageFeatureVectors, right.UsageFeatureVectors);


        protected override void UpdateSimilarityMatrix(Node item1, Node item2, ClusterNode clusterNode,
            SimilarityMatrix matrix)
        {
            _featureVectors.Add(clusterNode, _featureVectors[item1]
                .Merge(_featureVectors[item2]));

            foreach (var node in Nodes.Where(node => node != item1 && node != item2))
            {
                matrix.Add(node, clusterNode, Similarity(node, clusterNode));
                matrix.Remove(node, item1);
                matrix.Remove(node, item2);
            }

            matrix.Remove(item1, item2);

            _featureVectors.Remove(item1);
            _featureVectors.Remove(item2);
        }
    }
}