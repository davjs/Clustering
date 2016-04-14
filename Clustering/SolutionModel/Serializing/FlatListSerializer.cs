using System.Collections.Generic;
using System.Linq;

namespace Clustering.SolutionModel.Serializing
{
    public static class FlatListSerializer
    {
        public static string EncodeList(IEnumerable<FlatEntry> entries)
        {
            return string.Join("", entries.Select(EncodeSingleEntry));
        }

        private static string EncodeSingleEntry(FlatEntry entry)
        {
            return EncodeNodeName(entry.name) + EncodeChildren(entry.childData);
        }

        private static string EncodeChildren(IReadOnlyCollection<string> childData)
            =>
                childData.Any()
                    ? string.Join("\n", childData.OrderBy(x => x)) + "\n"
                    : "";

        private static string EncodeNodeName(string name)
        {
            return "@" + name + ":\n";
        }
    }
}