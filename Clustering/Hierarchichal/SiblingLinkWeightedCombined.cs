using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;

namespace Clustering.Hierarchichal
{
    public class SiblingLinkWeightedCombined : IClusteringAlgorithm
    {
        public ISet<Node> Cluster(ISet<Node> nodes, ISet<DependencyLink> edges)
        {
            // Set to return
            var toReturn = nodes.ToMutableSet();
            while(true)
            {
                // ---- FIND CLUSTER PAIR

                var allGroups = edges.GroupBy(x => x.Dependency);
                // We only care about dependency pattern with more 1 dependor
                var groups = allGroups.Where(x => x.Count() > 1);
                
                // Take the first possible pattern
                var first = groups.FirstOrDefault();
                if (first == null)
                    break;
                // Get the children nodes that should be in the new cluster
                var children = first
                    .Select(dependencyLink => dependencyLink.Dependor).ToSet();

                // ---- CLUSTER CREATION

                // Create a new cluster, adding the childs
                var cluster = new ClusterNode().WithChildren(children);
                // Add the new cluster to our set
                toReturn.Add(cluster);
                // Remove the previous nodes that are now inside the cluster node
                toReturn.ExceptWith(children);
                // Recreate the dependency links as one from cluster -> dependency
                var newLink = new DependencyLink(cluster, first.Key);
                // Remove the old dependency links as they are now represented by the cluster link
                edges = edges.Except(first).Union(newLink).ToSet();
            }
            return toReturn;
        }
    }
}