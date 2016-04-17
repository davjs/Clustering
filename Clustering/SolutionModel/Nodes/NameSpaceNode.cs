using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public IEnumerable<NameSpaceNode> Leafnamespaces()
        {
            if (IsLeafNamespace())
                yield return this;
            else
            {
                foreach (var nSpace in Children.OfType<NameSpaceNode>().SelectMany(x => x.Leafnamespaces()))
                    yield return nSpace;
            }
        }

        private bool IsLeafNamespace()
            => Children.All(x => x is ClassNode);
        
    }
}