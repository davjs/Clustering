using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    public class NamedNode : Node
    {
        public NamedNode(string name, IEnumerable<Node> children = null, Node parent = null)
            : base(name, children, parent)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children) =>
            new NamedNode(Name, children, Parent);

        public override Node WithParent(Node parent) =>
            new NamedNode(Name, Children, parent);
    }
}