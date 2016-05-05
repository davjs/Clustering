using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Clustering.SolutionModel.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreLinq;

namespace Clustering.SolutionModel
{
    public static class SemanticModelWalker
    {
        public static IEnumerable<SymbolNode> GetSymbolTreeFromModels(IEnumerable<SemanticModel> semanticModels)
        {
            var locations = semanticModels.SelectMany(
                model =>
                    model.SyntaxTree.GetRoot()
                        .ChildNodes().OfType<NamespaceDeclarationSyntax>()
                        .Select(syntaxNode => new SymbolLocation(model, syntaxNode))).ToImmutableHashSet();

            // Remove all that have a parent
            var foundMatchingParent = locations
                .ToLookup(
                    possibleChild =>
                        locations.Any(
                            possibleParent => SymbolEquals(possibleChild.Symbol.ContainingSymbol, possibleParent.Symbol)));

            var unMatchedParents = foundMatchingParent[false].Select(x => x.Symbol as INamespaceSymbol).ToSet();

            var nextLevelTargets =
                foundMatchingParent[true].Union(foundMatchingParent[false].SelectMany(x => x.GetChildSymbols()))
                    .ToList();
            
            var stillHasParents = foundMatchingParent[false].Where(IsInsideNonGlobalNamespace).ToList();

            var rootLevelTargets = unMatchedParents
                .Except(stillHasParents.Symbols().Cast<INamespaceSymbol>()).ToMutableSet();
            
            //Unfortunately we need to walk up namespace nodes that doesn't have their own dedicated declaration
            foreach (var it in stillHasParents.ToList())
            {
                var mayHaveParent= it;
                while (mayHaveParent.ParentNamespace().IsInsideNonGlobalNamespace())
                {
                    var didHaveParent = new SymbolWithoutLocation(mayHaveParent.ParentNamespace());
                    if(!stillHasParents.Any(x => SymbolEquals(x.Symbol, didHaveParent.Symbol)))
                        stillHasParents.Add(didHaveParent);
                    mayHaveParent = didHaveParent;
                }
                // Found a new unique namespace root, add it to current level
                var parentNamespace = mayHaveParent.ParentNamespace();
                if (!rootLevelTargets.Any(x => SymbolEquals(x, parentNamespace)))
                    rootLevelTargets.Add(parentNamespace);
            };

            IEnumerable<SymbolNode> nextLevelResults = new List<SymbolNode>();
            if(nextLevelTargets.Any())
            {
                nextLevelResults = GetNodesForNextLevel(rootLevelTargets.Cast<INamespaceOrTypeSymbol>().ToSet()
                    , nextLevelTargets.Union(stillHasParents));
            }

            return rootLevelTargets.Select(symbol =>
                new NameSpaceNode(symbol)
                    .WithChildren(nextLevelResults.Where(x => x.Symbol.IsChildOf(symbol)).Cast<Node>().ToSet()) as SymbolNode);
        }

        private static bool IsInsideNonGlobalNamespace(this SymbolLocation x)
            => x.Symbol.IsInsideNonGlobalNamespace();
        
        private static bool IsInsideNonGlobalNamespace(this ISymbol x)
            => x.ContainingNamespace != null &&
                   !x.ContainingNamespace.IsGlobalNamespace;

        public static ISet<INamespaceSymbol> ParentNamespaces (this IEnumerable<SymbolLocation> symbols)
            => symbols.Select(x => x.Symbol.ContainingNamespace).ToSet();

        public static IEnumerable<INamespaceOrTypeSymbol> Symbols(this IEnumerable<SymbolLocation> symbolLocations)
            => symbolLocations.Select(x => x.Symbol);

        private static IEnumerable<SymbolNode> GetNodesForNextLevel(ISet<INamespaceOrTypeSymbol> previousLevel,
            IEnumerable<SymbolLocation> possiblyNextLevel)
        {
            if (!previousLevel.Any())
                throw new NotImplementedException();
            var isCurrentLevel =
                possiblyNextLevel.ToLookup(x => x.Symbol.isChildOfAnyIn(previousLevel));
            //.Any(y => Equals(y, x.Symbol.ContainingSymbol)));
            var currentLevel = isCurrentLevel[true].ToList();
            // These might be subjects to deeper levels
            var nextLevel = isCurrentLevel[false].ToList();

            nextLevel = nextLevel.Union(currentLevel.SelectMany(x => x.GetChildSymbols())).ToList();

            var symbolsForThisLevel = DistinctSymbols(currentLevel.Select(x => x.Symbol));
            var children = new List<SymbolNode>();
            if (nextLevel.Any())
                children = GetNodesForNextLevel(symbolsForThisLevel, nextLevel).ToList();

            var firstLocationPerSymbol = symbolsForThisLevel
                .Select(x => currentLevel.First(y => SymbolEquals(y.Symbol, x)));

            return firstLocationPerSymbol.Select(parent => NodeFromLocation(parent)
                    .WithChildren(children.Where(child => child.Symbol.IsChildOf(parent.Symbol)).Cast<Node>().ToSet())
                    as SymbolNode);
        }

        private static ISet<INamespaceOrTypeSymbol> DistinctSymbols(this IEnumerable<INamespaceOrTypeSymbol> symbols)
        {
            var uniqueSymbols = new HashSet<INamespaceOrTypeSymbol>();
            // ReSharper disable once LoopCanBePartlyConvertedToQuery, IT CANNOT
            foreach (var symbol in symbols)
            {
                if (!uniqueSymbols.Any(x => SymbolEquals(x, symbol)))
                    uniqueSymbols.Add(symbol);
            }
            return uniqueSymbols;
        } 

        private static bool isChildOfAnyIn(this ISymbol child, ISet<INamespaceOrTypeSymbol> possibleParents)
            => possibleParents.Any(child.IsChildOf);

        private static bool IsChildOf(this ISymbol child, ISymbol possibleParent) 
            => SymbolEquals(possibleParent,child.ContainingSymbol);

        class SymbolEqualityComparer : IEqualityComparer<ISymbol>
        {
            public bool Equals(ISymbol x, ISymbol y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(ISymbol obj)
            {
                throw new NotImplementedException();
            }
        }

        public static bool SymbolEquals(ISymbol symbol,ISymbol symbol2)
        {
            var asnSpace = symbol as INamespaceSymbol;
            var asnSpace2 = symbol2 as INamespaceSymbol;
            //  None is namespace
            if (asnSpace == null && asnSpace2 == null)
                return Equals(symbol, symbol2);
            // Left is namespace
            if (asnSpace != null && asnSpace2 == null)
                return asnSpace.ConstituentNamespaces.Contains(symbol2);
            // Right is namespace
            if (asnSpace == null)
                return asnSpace2.ConstituentNamespaces.Contains(symbol);
            // Both namespaces
            return asnSpace.ConstituentNamespaces.Intersect(asnSpace2.ConstituentNamespaces).Any();
        }

        public class SymbolWithoutLocation : SymbolLocation
        {
            public SymbolWithoutLocation(INamespaceOrTypeSymbol symbol)
            {
                Symbol = symbol;
            }

            public override IEnumerable<SymbolLocation> GetChildSymbols()
                => new List<SymbolLocation>();
        }
        
        public class SymbolLocation
        {
            public readonly SemanticModel Document;
            public INamespaceOrTypeSymbol Symbol;
            public readonly SyntaxNode SyntaxNode;

            protected SymbolLocation() { }

            public SymbolLocation(SemanticModel document, SyntaxNode syntaxNode)
            {
                Debug.Assert(syntaxNode is NamespaceDeclarationSyntax || syntaxNode is ClassDeclarationSyntax);
                Document = document;
                SyntaxNode = syntaxNode;
                Symbol = Document.GetDeclaredSymbol(syntaxNode) as INamespaceOrTypeSymbol;
            }

            public virtual IEnumerable<SymbolLocation> GetChildSymbols() =>
                SyntaxNode.ChildNodes().Where(x =>
                    x is ClassDeclarationSyntax ||
                    x is NamespaceDeclarationSyntax)
                    .Select(x =>
                        new SymbolLocation(Document, x));

            public override string ToString()
                => Symbol + " : " + SyntaxNode.SyntaxTree.FilePath;

            public INamespaceSymbol ParentNamespace()
                => Symbol.ContainingNamespace;
        }

        private static SymbolNode NodeFromLocation(SymbolLocation location)
        {
            if (location.Symbol is INamespaceSymbol)
                return new NameSpaceNode(location.Symbol as INamespaceSymbol);
            if (location.Symbol is INamedTypeSymbol)
                return CreateClassNode(location.Document, location.SyntaxNode as ClassDeclarationSyntax);
            throw new NotImplementedException();
        }

        public static ClassNode CreateClassNode(SemanticModel model, ClassDeclarationSyntax c)
        {
            var declaredSymbol = model.GetDeclaredSymbol(c);
            var subnodes = c.DescendantNodes().ToList();
            var symbols = subnodes.Select(node => model.GetSymbolInfo(node).Symbol)
                .Except(declaredSymbol).Distinct();

            var dependencies = symbols.OfType<INamedTypeSymbol>();
            var nrOfMethods = subnodes.OfType<MethodDeclarationSyntax>().Count();

            IEnumerable<TypeSyntax> basetypes = new List<TypeSyntax>();
            if (c.BaseList != null && c.BaseList.Types.Any())
                basetypes = c.BaseList.Types.Select(x => x.Type);
            return new ClassNode(new ClassInfo(basetypes, nrOfMethods, dependencies, declaredSymbol,
                c.TypeParameterList));
        }
    }
}