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
    
    public class PathedNode : Node
    {
        public readonly string Path;

        public PathedNode(string name,string path, IEnumerable<Node> children = null)
            : base(name, children)
        {
            Path = path;
        }

        public override Node WithChildren(IEnumerable<Node> children) =>
            new PathedNode(Name,Path, children);
    }
}