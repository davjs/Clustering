using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    // A solution folder is a solution item that contains other solution items = projects or solution folders
    internal class SolutionFolderNode : SolutionItemContainer, ISolutionItem
    {
        public SolutionFolderNode(string Name,IEnumerable<Node> children = null,Node parent = null) : base(Name, children, parent)
        {

        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new SolutionFolderNode(Name,children);
        }
            
        public override Node WithParent(Node parent)
        {
            return new SolutionFolderNode(Name, Children,parent);
        }
    }
}