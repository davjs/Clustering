using System;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Serializing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building.TestExtensions;

namespace Tests.Building
{
    [TestClass]
    public class Rasterizing
    {
        [TestMethod]
        public void Write()
        {
            var solutionModel = SolutionModelBuilder.FromPath(Paths.Shared.ThisSolution);
            SolutionModelRasterizer.Write(solutionModel, AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\Rasterized\\");
        }

        [TestMethod]
        public void WriteAndRead()
        {
            var solutionModel = SolutionModelBuilder.FromPath(Paths.Shared.ThisSolution);
            var proj1 = solutionModel.Projects().Last();
            var dependencies = DependencyResolver.GetDependencies(proj1.Classes());
            var encodedString = Flat.Encode.HierarchicalGraph(proj1.Children,
            n => n.Children,
            n => n.Name,
            n => dependencies[n]);
            var decodedProj = GraphDecoder.Decode(encodedString);
            TreeAssert.TreeEquals(proj1.Children, decodedProj.Nodes);
        }

        [TestMethod]
        public void Read()
        {
            var text = @"
                @Clustering:
                @Clustering\ClusterBenchmarker:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal:
                @Clustering\Hierarchichal\ClusteringAlgorithm:
                Clustering\Hierarchichal\ClusterNode
                Clustering\Hierarchichal\FeatureVector
                Clustering\Hierarchichal\SimilarityMatrix
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal\ClusterNode:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal\FeatureVector:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal\SiblingLinkWeightedCombined:
                Clustering\Hierarchichal\ClusteringAlgorithm
                Clustering\Hierarchichal\ClusterNode
                Clustering\Hierarchichal\FeatureVector
                Clustering\Hierarchichal\SimilarityMatrix
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal\SimilarityMatrix:
                Clustering\Hierarchichal\UnorderedMultiKeyDict
                Clustering\SolutionModel\Nodes\Node
                @Clustering\Hierarchichal\UnorderedMultiKeyDict:
                @Clustering\SolutionModel:
                @Clustering\SolutionModel\DependencyLink:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\SolutionModel\DependencyResolver:
                Clustering\SolutionModel\DependencyLink
                Clustering\SolutionModel\Nodes\ClassNode
                Clustering\SolutionModel\Nodes\Node
                @Clustering\SolutionModel\Extensions:
                @Clustering\SolutionModel\Integration:
                @Clustering\SolutionModel\Integration\ProjectWrapper:
                @Clustering\SolutionModel\NodeExtensions:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\SolutionModel\Nodes:
                @Clustering\SolutionModel\Nodes\ClassInfo:
                @Clustering\SolutionModel\Nodes\ClassNode:
                Clustering\SolutionModel\Nodes\ClassInfo
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\SymbolNode
                @Clustering\SolutionModel\Nodes\NameSpaceNode:
                Clustering\SolutionModel\Nodes\ClassNode
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\SymbolNode
                @Clustering\SolutionModel\Nodes\Node:
                @Clustering\SolutionModel\Nodes\ProjectNode:
                Clustering\SolutionModel\Integration\ProjectWrapper
                Clustering\SolutionModel\Nodes\ClassNode
                Clustering\SolutionModel\Nodes\NameSpaceNode
                Clustering\SolutionModel\Nodes\Node
                @Clustering\SolutionModel\Nodes\SolutionFolderNode:
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\SolutionItemContainer
                @Clustering\SolutionModel\Nodes\SolutionItemContainer:
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\ProjectNode
                Clustering\SolutionModel\Nodes\SolutionFolderNode
                @Clustering\SolutionModel\Nodes\SolutionNode:
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\SolutionItemContainer
                @Clustering\SolutionModel\Nodes\SymbolNode:
                Clustering\SolutionModel\Nodes\Node
                @Clustering\SolutionModel\ProjectExtensions:
                @Clustering\SolutionModel\SemanticModelWalker:
                Clustering\SolutionModel\Nodes\ClassInfo
                Clustering\SolutionModel\Nodes\ClassNode
                Clustering\SolutionModel\Nodes\NameSpaceNode
                Clustering\SolutionModel\Nodes\SymbolNode
                @Clustering\SolutionModel\SemanticModelWalker\SymbolLocation:
                @Clustering\SolutionModel\Serializing:
                @Clustering\SolutionModel\Serializing\FlatEntry:
                @Clustering\SolutionModel\Serializing\FlatListSerializer:
                Clustering\SolutionModel\Serializing\FlatEntry
                @Clustering\SolutionModel\Serializing\GraphEncoder:
                Clustering\SolutionModel\Serializing\FlatEntry
                Clustering\SolutionModel\Serializing\FlatListSerializer
                @Clustering\SolutionModel\Serializing\TreeEncoder:
                Clustering\SolutionModel\Serializing\FlatEntry
                Clustering\SolutionModel\Serializing\FlatListSerializer
                @Clustering\SolutionModel\SolutionModelBuilder:
                Clustering\SolutionModel\Nodes\Node
                Clustering\SolutionModel\Nodes\ProjectNode
                Clustering\SolutionModel\Nodes\SolutionFolderNode
                Clustering\SolutionModel\Nodes\SolutionNode
                Clustering\SolutionModel\SemanticModelWalker";
            var projectModel = GraphDecoder.Decode(text);


            var clustering = projectModel.Nodes.First();

            clustering.Name.Should().Be("Clustering");
            clustering.Children.Should().Contain(x => x.Name == "ClusterBenchmarker");
        }
    }

}
