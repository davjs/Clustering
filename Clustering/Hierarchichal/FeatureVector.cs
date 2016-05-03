using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public class FeatureVector
    {
        public readonly int Total;
        private readonly Dictionary<Node, int> _dict;

        public FeatureVector(int total, Dictionary<Node, int> dict)
        {
            Total = total;
            _dict = dict;
        }

        public FeatureVector(ISet<Node> deps)
        {
            Total = 1;
            _dict = deps.ToDictionary(x => x, x => 1);
        }

        public int this[Node n] => _dict[n];
        
        public ISet<Node> Except(FeatureVector other) =>
            _dict.Keys.Except(other._dict.Keys).ToSet();

        public ISet<Node> Intersect(FeatureVector other) =>
            _dict.Keys.Intersect(other._dict.Keys).ToSet();

        public FeatureVector Merge(FeatureVector other)
        {
            var bothKeys = _dict.Keys.Union(other._dict.Keys);
            var newDict = bothKeys.ToDictionary(x => x, x => _dict.GetValueOrDefault(x, 0)
                                                             + _dict.GetValueOrDefault(x, 0));
            return new FeatureVector(Total + other.Total, newDict);
        }

    }
}