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