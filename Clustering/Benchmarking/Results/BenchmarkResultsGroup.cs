using System.Collections.Generic;

namespace Clustering.Benchmarking.Results
{
    public class BenchmarkResultsGroup
    {
        public string Header;
        public IEnumerable<BenchMarkResultsEntry> Entries;

        public BenchmarkResultsGroup(string header, IEnumerable<BenchMarkResultsEntry> entries)
        {
            Header = header;
            Entries = entries;
        }
    }
}