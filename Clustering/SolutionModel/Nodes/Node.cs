using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.SolutionModel.Nodes
{
    public abstract class Node
    {   
        public string Name { get; }
        public ISet<Node> Children { get; }

        protected Node(string name, IEnumerable<Node> children)
        {
            Name = name;
            if (children != null)
            {
                Children = children.ToSet();
            }
        }

        public override string ToString()
        {
            return Children.Any() ? $"{Name} = ({string.Join(",", Children)})" : Name;
        }

        //TODO: Make the method signature explicitly show the runtime return type, ie SolutionNode(x).With(y) returns a SolutionNode
        public abstract Node WithChildren(IEnumerable<Node> children);
    }
}