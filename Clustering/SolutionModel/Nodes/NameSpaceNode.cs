using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Clustering.SolutionModel.Nodes
{
    public class NameSpaceNode : SymbolNode
    {
        public override ISymbol Symbol { get; }

        public NameSpaceNode(ISymbol symbol, IEnumerable<Node> children = null, Node parent = null) : base(symbol.Name, children, parent)
        {
            Symbol = symbol;
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new NameSpaceNode(Symbol, children);
        }

        public override Node WithParent(Node parent)
        {
            return new NameSpaceNode(Symbol, Children, parent);
        }

        public IEnumerable<ClassNode> Classes()
        {
            foreach (var node in Children)
            {
                if (node is ClassNode)
                    yield return node as ClassNode;
                else
                {
                    Debug.Assert(node is NameSpaceNode);
                    foreach (var classNode in (node as NameSpaceNode).Classes())
                        yield return classNode;
                }
            }
        }
    }

    public class ClusterNode : Node
    {
        public ClusterNode(IEnumerable<Node> children = null, Node parent = null) : base("$", children, parent)
        {
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new ClusterNode(children,Parent);
        }

        public override Node WithParent(Node parent)
        {
            return new ClusterNode(Children, parent);
        }
    }
}