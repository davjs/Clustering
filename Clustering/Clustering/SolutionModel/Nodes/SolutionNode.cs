using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    public class SolutionNode : SolutionItemContainer
    {
        public SolutionNode(string name, IEnumerable<Node> children = null, Node parent = null) : base(name, children,parent)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new SolutionNode(Name, children);
        }

        public override Node WithParent(Node parent)
        {
            return new SolutionNode(Name,Children,parent);
        }
    }
}