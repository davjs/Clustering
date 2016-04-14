// This project can output the Class library as a NuGet Package.
// To enable this option, right-click on the project and select the Properties menu item.
//In the Build tab select "Produce outputs on build".

using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.SolutionModel.Serializing
{
    public static class TreeEncoder
    {

        public delegate IEnumerable<T> GetChildren<T>(T parent);
        public delegate IReadOnlyCollection<string> GetDataList<in T>(T parent);

        public static IEnumerable<FlatEntry> FlattenTree<T>(this IEnumerable<T> nodes,
            GetChildren<T> childAccecor, Func<T, string> nameAccessor, GetDataList<T> dataAccessor, string path = "")
        {
            var entries = new List<FlatEntry>();
            foreach (var node in nodes.OrderBy(nameAccessor))
            {
                entries.Add(new FlatEntry
                {
                    name = path + nameAccessor(node),
                    childData = dataAccessor(node)
                });
                var newPath = path + nameAccessor(node) + "\\";
                entries.AddRange(childAccecor(node).FlattenTree(childAccecor, nameAccessor, dataAccessor, newPath));
            }
            return entries;
        }


        public static string EncodeTree<T>(this IEnumerable<T> nodes,
        GetChildren<T> childAccecor, Func<T, string> nameAccessor, GetDataList<T> dataAccessor, string path = "")
        {
            var listToEncode = nodes.FlattenTree(childAccecor, nameAccessor, dataAccessor);
            return FlatListSerializer.EncodeList(listToEncode);
        }
    }
}