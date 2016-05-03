using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly SeparatedSyntaxList<TypeParameterSyntax> _genericParameters;

        public ClassInfo(IEnumerable<TypeSyntax> baseClasses, int nrOfMethods, IEnumerable<INamedTypeSymbol> symbolDependencies, ISymbol symbol, 
            TypeParameterListSyntax genericParameters)
        {
            BaseClasses = baseClasses;
            NrOfMethods = nrOfMethods;
            SymbolDependencies = symbolDependencies;
            Symbol = symbol;
            _genericParameters = genericParameters?.Parameters 
                ?? new SeparatedSyntaxList<TypeParameterSyntax>();
        }

        public String GetName()
        {
            if (_genericParameters.Any())
                return $"{Symbol.Name}<{string.Join(",",_genericParameters)}>";
            return Symbol.Name;
        }
    }

    public class ClassNode : SymbolNode
    {
        public readonly ClassInfo ClassProperties;

        public ClassNode(ClassInfo classInfo
            ,IEnumerable<Node> children = null) : base(classInfo.GetName(),children)
        {
            ClassProperties = classInfo;
        }

        public override Node WithChildren(IEnumerable<Node> children)
        {
            return new ClassNode(ClassProperties, children);
        }

        public override ISymbol Symbol => ClassProperties.Symbol;
    }
}