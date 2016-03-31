using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public class SiblingLinkWeightedCombined : IClusteringAlgorithm
    {
        public IEnumerable<Node> Cluster(IEnumerable<Node> nodes, IEnumerable<DependencyLink> edges)
        {
            var newList = nodes.ToList();
            var list = new List<IGrouping<Node, DependencyLink>>();
            while(true)
            {
                list = edges.GroupBy(x => x.Dependency).ToList();
                list.RemoveAll(x => x.Count() <= 1);
                var first = list.FirstOrDefault();
                if (first == null) break;

                var children = new List<Node>();
                foreach (DependencyLink link in first.ToList())
                {
                    children.Add(link.Dependor);
                }
                var cluster = new ClusterNode().WithChildren(children);
                newList.Add(cluster);
                newList.RemoveAll(x => children.Contains(x));
                var newLink = new DependencyLink(cluster, first.Key);
                edges = edges.Except(first).Union(newLink);
            }
            return newList;
        }
    }
}