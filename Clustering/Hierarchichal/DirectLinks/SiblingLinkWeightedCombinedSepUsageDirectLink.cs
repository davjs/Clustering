using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal.DirectLinks
{
    public class SiblingLinkWeightedCombinedSepUsageDirectLink : ClusteringAlgorithm
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

        private Dictionary<Node, FeatureVectorContainer> featureVectors;

        protected override void Setup(ISet<Node> nodes, ILookup<Node, Node> dependencies)
        {
            var dep2 = dependencies.ToDictionary(x => x.Key, x => new HashSet<Node>(x));

            var usages = nodes.ToDictionary(
                dependency => dependency,
                dependency => new List<Node>());

            foreach (var edge in dependencies)
            {
                var dependent = edge.Key;
                foreach (var dependency in edge)
                {
                    // TODO: Is this check really needed ?
                    if (!usages.ContainsKey(dependency))
                        usages[dependency] = new List<Node>();
                    usages[dependency].Add(dependent);
                }
            }

            featureVectors = nodes.ToDictionary(
                node => node,
                node => new FeatureVectorContainer(
                    dependencyFeatureVector: new FeatureVector(dependencies[node].ToSet()),
                    usageFeatureVector: new FeatureVector(usages[node].ToSet())));
        }

        protected override double Similarity(Node left, Node right)
        {
            var vecLeft = featureVectors[left];
            var vecRight = featureVectors[right];
            if (DirectLink(left, right) || DirectLink(right,left))
                return 1;

            return Similarity(vecLeft, vecRight);
        }

        private bool DirectLink(Node left, Node right)
        {
            var vecLeft = featureVectors[left];
            var vecRight = featureVectors[right];

            var usesLeft = vecLeft.UsageFeatureVectors.Positives().ToHashSet();
            var usesRight = vecRight.UsageFeatureVectors.Positives().ToHashSet();

            var allRightUsersUsesLeft = usesRight.IsSubsetOf(usesLeft) && usesRight.Count != 0;
            var leftDependsOnRight = vecLeft.DependencyFeatureVectors.Has(right);
            if (leftDependsOnRight && allRightUsersUsesLeft)
            {
                return true;
            }
            return false;
        }

        private double Similarity(FeatureVectorContainer left, FeatureVectorContainer right)
            => Ellenberg(left.DependencyFeatureVectors, right.DependencyFeatureVectors)
               + Ellenberg(left.UsageFeatureVectors, right.UsageFeatureVectors);


        protected override void UpdateSimilarityMatrix(Node item1, Node item2, ClusterNode clusterNode,
            SimilarityMatrix matrix)
        {
            featureVectors.Add(clusterNode, featureVectors[item1]
                .Merge(featureVectors[item2]));

            foreach (var node in Nodes.Where(node => node != item1 && node != item2))
            {
                matrix.Add(node, clusterNode, Similarity(node, clusterNode));
                matrix.Remove(node, item1);
                matrix.Remove(node, item2);
            }

            matrix.Remove(item1, item2);

            featureVectors.Remove(item1);
            featureVectors.Remove(item2);
        }
    }
}