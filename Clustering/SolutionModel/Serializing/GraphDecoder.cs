using System;
using System.Collections.Generic;
using System.Linq;
using Clustering.SolutionModel.Nodes;

namespace Clustering.SolutionModel.Serializing
{
    public class ProjectTreeWithDependencies
    {
        public IEnumerable<Node> Nodes;
        public ILookup<Node, Node> Edges;

        public ProjectTreeWithDependencies(IEnumerable<Node> nodes, ILookup<Node, Node> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }

    public static class GraphDecoder
    {
        public static ProjectTreeWithDependencies Decode(string text)
        {
            var flats = FlatListSerializer.DecodeFlatlist(text);
            var nodesByPath = new Dictionary<string, Node>();
            var nodes = GetTreeFromFlatlist(flats,0,nodesByPath).ToList();
            var deps = GetDependenciesFromFlatList(flats, nodesByPath);
            return new ProjectTreeWithDependencies(nodes, deps);
        }

        private static ILookup<Node,Node> GetDependenciesFromFlatList(IEnumerable<FlatEntry> flats, Dictionary<string, Node> nodesByPath)
        {
            var edgeList = 
                (from flatEntry in flats
                from child in flatEntry.childData
                select Tuple.Create(nodesByPath[flatEntry.path], nodesByPath[child])).ToList();
            return edgeList.ToLookup(x => x.Item1, x => x.Item2);
        }

        private static string GetNameFromPath(string path)
        {
            var splits = path.Split('\\');
            splits.Take(splits.Length - 1);
            return splits.Last();
        }

        private static IEnumerable<Node> GetTreeFromFlatlist(IEnumerable<FlatEntry> descendants, int depth, IDictionary<string, Node> nodesByPath)
        {
            var isDirectChildren = descendants.ToLookup(X => X.path.Count(c => c == '\\') == depth);
            var notDirectChildren = isDirectChildren[false].ToMutableSet();
            var directChildren = isDirectChildren[true];
            foreach (var directChild in directChildren)
            {
                var path = directChild.path;
                var name = GetNameFromPath(path);
                var topNode = new NamedNode(name);
                var subDescendants = notDirectChildren.Where(x => x.path.StartsWith(path + "\\")).ToList();
                notDirectChildren.ExceptWith(subDescendants);
                var withChildren = topNode.WithChildren(GetTreeFromFlatlist(subDescendants, depth + 1, nodesByPath));
                nodesByPath.Add(path,withChildren);
                yield return withChildren;
            }
            // Should never happen, lets keep this for assurance a while
            if(notDirectChildren.Any())
                throw new NotImplementedException();
        }
    }
}
