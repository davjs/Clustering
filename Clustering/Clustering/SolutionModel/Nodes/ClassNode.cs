using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Clustering.SolutionModel.Nodes
{
    public class ClassInfo
    {
        public readonly IEnumerable<TypeSyntax> BaseClasses;
        public readonly int NrOfMethods;
        public readonly IEnumerable<INamedTypeSymbol> SymbolDependencies;
        public readonly ISymbol Symbol;

        public ClassInfo(IEnumerable<TypeSyntax> baseClasses, int nrOfMethods, IEnumerable<INamedTypeSymbol> symbolDependencies, ISymbol symbol)
        {
            BaseClasses = baseClasses;
            NrOfMethods = nrOfMethods;
            SymbolDependencies = symbolDependencies;
            Symbol = symbol;
        }
    }

    public class ClassNode : SymbolNode
    {
        public readonly ClassInfo ClassProperties;

        public ClassNode(ClassInfo classInfo
            ,IEnumerable<Node> children = null, Node parent = null) : base(classInfo.Symbol.Name,children,parent)
        {
            ClassProperties = classInfo;
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new ClassNode(ClassProperties, children);
        }

        public override Node WithParent(Node parent)
        {
            return new ClassNode(ClassProperties, Children,parent);
        }

        public override ISymbol Symbol => ClassProperties.Symbol;
    }
}