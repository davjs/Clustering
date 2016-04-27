using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Clustering.SolutionModel.Nodes
{
    public abstract class SymbolNode : Node
    {
        public abstract ISymbol Symbol { get;}

        public SymbolNode(string name, IEnumerable<Node> children) : base(name, children)
        {
        }
    }
}