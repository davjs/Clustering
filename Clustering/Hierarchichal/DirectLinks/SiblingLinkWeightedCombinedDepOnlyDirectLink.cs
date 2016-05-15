using System;
using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal.DirectLinks
{
    public class SimilarityMatrix : UnorderedMultiKeyDict<Node, double>, ISimilarityMatrix
    {
        public KeyValuePair<Tuple<Node, Node>, double> GetMostSimilar() => this.MaxBy(x => x.Value);
    }

    public class SiblingLinkWeightedCombinedDepOnlyDirectLink : ClusteringAlgorithm
    {
        private Dictionary<Node, FeatureVector> _featureVectors;
        private Dictionary<Node, HashSet<Node>> _usages;
        private Dictionary<Node, HashSet<Node>> _dependencies;

        protected override void Setup(ISet<Node> nodes, ILookup<Node, Node> edges)
        {
            _dependencies = nodes.ToDictionary(x => x, x => new HashSet<Node>(edges[x]));

            _usages = nodes.ToDictionary(
                dependency => dependency,
                dependency => new HashSet<Node>());

            foreach (var edge in _dependencies)
            {
                var dependent = edge.Key;
                foreach (var dependency in edge.Value)
                {
                    // TODO: Is this check really needed ?
                    if (!_usages.ContainsKey(dependency))
                        _usages[dependency] = new HashSet<Node>();
                    _usages[dependency].Add(dependent);
                }
            }

            _featureVectors = nodes.ToDictionary(
                dependor => dependor,
                dependor => new FeatureVector(_dependencies[dependor]));
        }

        protected override double Similarity(Node left, Node right)
        {
            if (D(left, right) || D(right,left))
                return 1;
            return Ellenberg(_featureVectors[left], _featureVectors[right]);
        }

        private bool D(Node left, Node right)
        {
            var usesLeft = _usages[left];
            var usesRight = _usages[right];

            var allRightUsersUsesLeft = usesRight.IsSubsetOf(usesLeft) && usesRight.Count != 0;
            var leftDependsOnRight = _dependencies[left].Contains(right);
            return leftDependsOnRight && allRightUsersUsesLeft;
        }


        protected override void UpdateSimilarityMatrix(Node item1, Node item2, ClusterNode clusterNode,
            SimilarityMatrix matrix)
        {
            _featureVectors.Add(clusterNode, _featureVectors[item1]
                .Merge(_featureVectors[item2]));

            _usages.Add(clusterNode, _usages[item1].Union(_usages[item2]).ToHashSet());
            _dependencies.Add(clusterNode, _dependencies[item1].Union(_dependencies[item2]).ToHashSet());

            foreach (var node in Nodes.Where(node => node != item1 && node != item2))
            {
                matrix.Add(node, clusterNode, Similarity(node,
                    clusterNode));
                matrix.Remove(node, item1);
                matrix.Remove(node, item2);
            }

            matrix.Remove(item1, item2);

            _featureVectors.Remove(item1);
            _featureVectors.Remove(item2);
            
            _usages.Remove(item1);
            _usages.Remove(item2);
            _dependencies.Remove(item1);
            _dependencies.Remove(item2);
        }
    }
}