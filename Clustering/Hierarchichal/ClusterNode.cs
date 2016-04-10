using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreLinq;

namespace Clustering.Hierarchichal
{
    public class FeatureVector
    {
        public readonly int Total;
        private readonly Dictionary<Node, int> _dict;

        public FeatureVector(int total, Dictionary<Node, int> dict)
        {
            Total = total;
            _dict = dict;
        }

        public FeatureVector(IEnumerable<Node> deps)
        {
            Total = 1;
            _dict = deps.ToDictionary(x => x, x => 1);
        }

        public int this[Node n] => _dict[n];

        public ISet<Node> Union(FeatureVector other) =>
            _dict.Keys.Union(other._dict.Keys).ToSet();
        
        public ISet<Node> Except(FeatureVector other) =>
            _dict.Keys.Except(other._dict.Keys).ToSet();

        public ISet<Node> Intersect(FeatureVector other) =>
            _dict.Keys.Intersect(other._dict.Keys).ToSet();

        public FeatureVector Merge(FeatureVector other)
        {
            var bothKeys = _dict.Keys.Union(other._dict.Keys);
            var newDict = bothKeys.ToDictionary(x => x, x => _dict.GetValueOrDefault(x, 0)
                                               + _dict.GetValueOrDefault(x, 0));
            return new FeatureVector(Total + other.Total, newDict);
        }

    }

    public class ClusterNode : Node
    {
        private readonly int _total = 0;

        public ClusterNode(IEnumerable<Node> children = null, Node parent = null) : base("$", children, parent)
        {
            
        }

        public ClusterNode(Node a, Node b) : base("$", new List<Node> {a, b}, null)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new ClusterNode(children, Parent);
        }

        public override Node WithParent(Node parent)
        {
            return new ClusterNode(Children, parent);
        }
    }
}