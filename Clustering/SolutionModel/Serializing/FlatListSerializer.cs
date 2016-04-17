using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Clustering.SolutionModel.Serializing
{
    public static class FlatListSerializer
    {
        public static string EncodeList(IEnumerable<FlatEntry> entries) =>
            string.Join("", entries.Select(EncodeSingleEntry));
        

        private static string EncodeSingleEntry(FlatEntry entry)
            => EncodeNodeName(entry.path) + EncodeChildren(entry.childData);
        

        private static string EncodeChildren(IReadOnlyCollection<string> childData)
            =>
                childData.Any()
                    ? string.Join("\n", childData.OrderBy(x => x)) + "\n"
                    : "";

        private static string EncodeNodeName(string name) => "@" + name + ":\n";
        

        public static IReadOnlyCollection<FlatEntry> DecodeFlatlist(string text)
        {
            text = text.Trim();
            text = Regex.Replace(text, @"[ \r]", "");

            var entryGroups = text.Split('@').Skip(1);

            return (from entryGroup in entryGroups
                    let entriesInGroup = entryGroup.Split('\n').ToList()
                    let nodeName = entriesInGroup.First().TrimEnd(':')
                    let childs = entriesInGroup.Skip(1) // Ignore first and last newline
                        .Take(entriesInGroup.Count - 2)
                    select new FlatEntry { path = nodeName, childData = childs.ToList() }).ToList();
        }

    }
}