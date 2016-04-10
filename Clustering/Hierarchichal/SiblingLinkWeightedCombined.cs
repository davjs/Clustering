using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal
{
    public class UnorderedMultiKeyDict<TKey, TValue> : Dictionary<Tuple<TKey, TKey>, TValue>
    {
        //protected Dictionary<Tuple<TKey, TKey>, TValue> _dict;

        private Tuple<TKey, TKey> GetOrderedTuple(TKey key1, TKey key2) =>
            key1.GetHashCode() > key2.GetHashCode() ? 
            Tuple.Create(key1,key2) : 
            Tuple.Create(key2, key1);

        public void Add(TKey key1, TKey key2 ,TValue value)
        {
            Add(GetOrderedTuple(key1,key2),value);
        }

        public TValue Get(TKey key1, TKey key2) =>
            this[GetOrderedTuple(key1, key2)];
    }

    public class SimilarityMatrix : UnorderedMultiKeyDict<Node, double>, ISimilarityMatrix
    {
        //private readonly UnorderedMultiKeyDict<Node, double> dict = new UnorderedMultiKeyDict<Node, double>();
        public Tuple<Node, Node> GetMostSimilar() => this.MaxBy(x => x.Value).Key;
    }
    

    public class SiblingLinkWeightedCombined : ClusteringAlgorithm
    {
        public override ISimilarityMatrix CreateSimilarityMatrix(ISet<Node> nodes, ISet<DependencyLink> edges)
        {
            var featureVectors = edges.GroupBy(x => x.Dependor)
                .ToDictionary(deps => deps.Key, deps => new FeatureVector(deps.Select(x => x.Dependency)));

            var simMatrix = new SimilarityMatrix();

            var pairs = (from x in nodes
                from y in nodes
                where x.GetHashCode() > y.GetHashCode()
                select new {x = x, y = y});

            foreach (var pair in pairs)
            {
                simMatrix.Add(pair.x,pair.y,Similarity(featureVectors[pair.x],featureVectors[pair.y]));
            }

            return simMatrix;
        }

        public override double Similarity(FeatureVector a, FeatureVector b)
        {
            var both = a.Intersect(b);
            var onlyA = a.Except(b);
            var onlyB = b.Except(a);

            var MaHalf = both.Sum(x => a[x] / a.Total 
                                    + b[x] / b.Total) * 0.5;

            var Mb = onlyA.Sum(x => a[x]) / a.Total;
            var Mc = onlyB.Sum(x => b[x]) / b.Total;

            return MaHalf/(MaHalf + Mb + Mc);
        }
    }
}