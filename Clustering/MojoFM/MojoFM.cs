using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.MojoFM
{
    public class MojoFM
    {
        public static double Calc(IEnumerable<Node> inA, IEnumerable<Node> inB)
        {
            // Calculating mno(A, B):
            // For each node in A, assign which cluster the node is in in B
            // For each cluster in B, create a group Gi
            // For each cluster in A, cluster Ai belongs in the group which it has most members.
            // If there is ambiguity, maximize the number of non-empty groups

            // Begin of Move and Join
            // For each empty group, create an empty cluster
            // Move every node to the group it was assigned
            // Count the number of move operations, save as M
            // Join all clusters belonging to the same group
            // Count number of joins
            // After this we should have groups containing only a single cluster
            // Total cost of algorithm is M + (clusters in A) - (# of non empty groups)

            // Calculating max(mno(VA, B))
            // max(mno(VA, B)) == max(mno(B, VA))
            
            // MoJoFM(M) = 1 - mno(A,B) / max(mno(any_A, B)) * 100 %

            var a = new List<Node>(inA);
            var b = new List<Node>(inB);
            // Clusters in A
            int l = a.Count();
            // Clusters in B
            int m = b.Count();
            // Location of nodes in cluster B
            var dictB = new Dictionary<string, Node>();
            
            foreach (var clusterInB in b)
            {
                foreach (var nodeInCluster in clusterInB.Children)
                {
                    dictB.Add(nodeInCluster.Name, clusterInB);
                }
            }
            
            // Bipartite Matching
            var graph = new BipartiteGraph(l + m, l);

            for (int i = 0; i < l; i++) // For each cluster in A
            {
                var children = a[i].Children.ToList();
                foreach (var node in children)
                {
                    int indexInB = b.IndexOf(dictB[node.Name]);
                    graph.AddEdge(i, l + indexInB);
                }
            }

            graph.Matching();

            var mappings = new Dictionary<Node, int>();
            var groupscount = new int[m];
            var grouptags = new Dictionary<int, Node>();
            for (int i = 0; i < l; i++)
            {
                grouptags.Add(i, null);
            }

            for (int i = l; i < l + m; i++)
            {
                if (graph.Vertices[i].Matched)
                {
                    int index = graph.AdjList[i][0];
                    mappings.Add(a[index], i - l); // a[index] match to group (i - l);
                }
            }


            // Calculate cost
            int moves = 0;
            int nonempty = 0;

            for (int i = 0; i < l; i++)
            {
                int index = mappings[a[i]];
                if (groupscount[index] == 0)
                {
                    nonempty++;
                }

                if (grouptags[index] == null)
                {
                    grouptags[index] = a[i];
                }

                groupscount[index]++;

                // Get the count of the most common tag in a
                int maxCount = a[i].Children.Select(node => b.IndexOf(dictB[node.Name])).GroupBy(x => x).MaxBy(x => x.Count()).Count();

                moves += a[i].Children.Count - maxCount;
            }
            long totalcost = moves + l - nonempty;
            

            // Calc max distance
            int groupnumber = 0;
            int objectsInA = a.SelectMany(list => list.Children).Count();
            var cardinalitiesInB = b.Select(cluster => cluster.Children.Count).ToList();
            cardinalitiesInB.Sort();

            foreach (int i in cardinalitiesInB)
            {
                if (groupnumber < i) groupnumber++;
            }
            long maxDis = objectsInA - groupnumber;

            return Math.Round((1 - (double) totalcost/(double) maxDis)*10000)/100;
        }
    }
}
