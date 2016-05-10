using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clustering.Benchmarking.Results
{
    public class ResultsFile
    {
        private readonly List<string> _lines = new List<string>();

        public void AddResultGroup(BenchmarkResultsGroup group)
        {
            AddHeader(group.Header).WithResults(group.Entries);
        }

        public void AddResultGroups(IEnumerable<BenchmarkResultsGroup> groups)
        {
            foreach (var @group in groups)
                AddResultGroup(group);
        }

        public ResultsFile WithResults(IEnumerable<BenchMarkResultsEntry> entries)
        {
            _lines.AddRange(entries.Select(x => x.ToString()));
            return this;
        }

        public ResultsFile AddEntry(BenchMarkResultsEntry entry)
        {
            _lines.Add(entry.ToString());
            return this;
        }
        

        public void Write(string path)
        {
            File.WriteAllLines(path, _lines);
        }

        public ResultsFile AddHeader(string name)
        {
            _lines.Add($"  -- {name} -- :");
            return this;
        }
    }
}