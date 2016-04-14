using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.SolutionModel.Nodes
{
    public abstract class Node
    {   
        public string Name { get; }
        public ISet<Node> Children { get; }
        public Node Parent { get; }

        protected Node(string name, IEnumerable<Node> children, Node parent)
        {
            Name = name;
            Parent = parent;
            if (children != null)
            {
                Children = children.Select(x => x.WithParent(this)).ToSet();
            }
        }

        public override string ToString()
        {
            return Children.Any() ? $"{Name} = ({string.Join(",", Children)})" : Name;
        }

        //TODO: Make the method signature explicitly show the runtime return type, ie SolutionNode(x).With(y) returns a SolutionNode
        public abstract Node WithChildren(IEnumerable<Node> children);
        public abstract Node WithParent(Node parent);
    }
}