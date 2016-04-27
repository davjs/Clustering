using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Clustering.SolutionModel.Nodes
{
    public class NameSpaceNode : SymbolNode
    {
        public override ISymbol Symbol { get; }

        public NameSpaceNode(ISymbol symbol, IEnumerable<Node> children = null) : base(symbol.Name, children)
        {
            Symbol = symbol;
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new NameSpaceNode(Symbol, children);
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
}