using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    public class SolutionNode : SolutionItemContainer
    {
        public SolutionNode(string name, IEnumerable<Node> children = null, Node parent = null) : base(name, children)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new SolutionNode(Name, children);
        }
    }
}