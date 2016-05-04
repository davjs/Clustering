using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Clustering.SolutionModel.Nodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreLinq;

namespace Clustering.Hierarchichal
{
    public class ClusterNode : Node
    {
        public readonly double _similarity;
        private readonly int _total = 0;

        public ClusterNode(IEnumerable<Node> children = null) : base("$", children)
        {

        }

        public ClusterNode(double similarity, Node a, Node b) : base($"$({similarity})", new List<Node> {a, b})
        {
            _similarity = similarity;
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new ClusterNode(children);
        }
    }

    public class SingletonCluster : ClusterNode
    {
        public SingletonCluster(Node child) : base(new HashSet<Node> {child})
        {

        }
    }
}