using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Clustering.SolutionModel.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Clustering.SolutionModel
{
    public static class SemanticModelWalker
    {
        public static IEnumerable<SymbolNode> GetClassTree(IEnumerable<SemanticModel> semanticModels)
        {
            var locations = semanticModels.SelectMany(
                model =>
                    model.SyntaxTree.GetRoot()
                        .ChildNodes().OfType<NamespaceDeclarationSyntax>()
                        .Select(syntaxNode => new SymbolLocation(model, syntaxNode))).ToImmutableHashSet();

            // Remove all that have a parent
            var hasParents = locations
                .ToLookup(
                    possibleChild =>
                        locations.Any(
                            possibleParent => Equals(possibleChild.Symbol.ContainingSymbol, possibleParent.Symbol)));

            var curLevel = hasParents[false].ToList();
            var nextLevel = hasParents[true].Union(curLevel.SelectMany(x => x.GetChildSymbols()));

            var children =
                GetNodesForNextLevel(curLevel.Select(x => x.Symbol).ToImmutableHashSet(),
                    nextLevel);
            
            var firstLocationPerSymbol = curLevel.GroupBy(x => x.Symbol).Select(x => x.First());

            return firstLocationPerSymbol.Select(location =>
                NodeFromLocation(location)
                    .WithChildren(children.Where(x => Equals(x.Symbol.ContainingSymbol, location.Symbol))) as SymbolNode);
        }

        private static IEnumerable<SymbolNode> GetNodesForNextLevel(ISet<INamespaceOrTypeSymbol> previousLevel,
            IEnumerable<SymbolLocation> possiblyNextLevel)
        {
            if (!previousLevel.Any())
                throw new NotImplementedException();
            var isCurrentLevel =
                possiblyNextLevel.ToLookup(x => previousLevel.Contains(x.Symbol.ContainingSymbol));
            //.Any(y => Equals(y, x.Symbol.ContainingSymbol)));
            var currentLevel = isCurrentLevel[true].ToList();
            // These might be subjects to deeper levels
            var nextLevel = isCurrentLevel[false].ToList();

            nextLevel = nextLevel.Union(currentLevel.SelectMany(x => x.GetChildSymbols())).ToList();

            var symbolsForThisLevel = currentLevel.Select(x => x.Symbol).Distinct();
            var children = new List<SymbolNode>();
            if (nextLevel.Any())
                children = GetNodesForNextLevel(symbolsForThisLevel.ToImmutableHashSet(), nextLevel).ToList();

            //Distinct by symbol, TODO: Add morelinq and use DistinctBy(x => x.Symbol)
            var firstLocationPerSymbol = currentLevel.GroupBy(x => x.Symbol).Select(x => x.First());

            return firstLocationPerSymbol.Select(location => NodeFromLocation(location)
                    .WithChildren(children.Where(x => Equals(x.Symbol.ContainingSymbol, location.Symbol)))
                    as SymbolNode);
        }

        public class SymbolLocation
        {
            public readonly SemanticModel Document;
            public readonly INamespaceOrTypeSymbol Symbol;
            public readonly SyntaxNode SyntaxNode;

            public SymbolLocation(SemanticModel document, SyntaxNode syntaxNode)
            {
                Debug.Assert(syntaxNode is NamespaceDeclarationSyntax || syntaxNode is ClassDeclarationSyntax);
                Document = document;
                SyntaxNode = syntaxNode;
                Symbol = Document.GetDeclaredSymbol(syntaxNode) as INamespaceOrTypeSymbol;
            }

            public IEnumerable<SymbolLocation> GetChildSymbols() =>
                SyntaxNode.ChildNodes().Where(x =>
                    x is ClassDeclarationSyntax ||
                    x is NamespaceDeclarationSyntax)
                    .Select(x =>
                        new SymbolLocation(Document, x));
        }

        private static SymbolNode NodeFromLocation(SymbolLocation location)
        {
            if (location.Symbol is INamespaceSymbol)
                return new NameSpaceNode(location.Symbol);
            if (location.Symbol is INamedTypeSymbol)
                return CreateClassNode(location.Document, location.SyntaxNode as ClassDeclarationSyntax);
            throw new NotImplementedException();
        }

        public static ClassNode CreateClassNode(SemanticModel model, ClassDeclarationSyntax c)
        {
            var declaredSymbol = model.GetDeclaredSymbol(c);
            var subnodes = c.DescendantNodes().ToList();
            var symbols = subnodes.Select(node => model.GetSymbolInfo(node).Symbol);

            var dependencies = symbols.OfType<INamedTypeSymbol>();
            var nrOfMethods = subnodes.OfType<MethodDeclarationSyntax>().Count();

            IEnumerable<TypeSyntax> basetypes = new List<TypeSyntax>();
            if (c.BaseList != null && c.BaseList.Types.Any())
                basetypes = c.BaseList.Types.Select(x => x.Type);
            return new ClassNode(new ClassInfo(basetypes, nrOfMethods, dependencies, declaredSymbol));
        }
    }
}