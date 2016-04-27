using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    public class NamedNode : Node
    {
        public NamedNode(string name, IEnumerable<Node> children = null)
            : base(name, children)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children) =>
            new NamedNode(Name, children);
    }
}