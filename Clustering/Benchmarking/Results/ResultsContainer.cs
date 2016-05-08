using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clustering.Benchmarking.Results
{
    public class ResultsContainer
    {
        private readonly Dictionary<Repository, Dictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>>> _repoScores;

        public ResultsContainer(
            Dictionary<Repository, Dictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>>> repoScores)
        {
            _repoScores = repoScores;
        }

        public void WriteToFolder(string outputfolder)
        {
            // Per repo file
            foreach (var repositoryBenchMarks in _repoScores)
            {
                var repository = repositoryBenchMarks.Key;
                var benchMarkByConfig = repositoryBenchMarks.Value;
                new RepoResultsFile(benchMarkByConfig)
                    .Write(outputfolder + $"{repository.Name}.Results");
            }

            // Average file
            var averageFile = new AverageResultsFile();
            foreach (var repositoryBenchMarks in _repoScores)
            {
                var repository = repositoryBenchMarks.Key;
                var perConfigBench = repositoryBenchMarks.Value;
                var averagePerConfigResults =
                    perConfigBench.Select(bench => new BenchMarkResultsEntry(bench.Key.Name, bench.Value.Average()));
                averageFile.AddRepoPerConfigAverages(repository.Name, averagePerConfigResults);
            }

            // Total averages
            var allPerConfigResults = _repoScores.SelectMany(x => x.Value);
            var byConfig = allPerConfigResults.ToLookup(x => x.Key, x => x.Value);
            var flattenedByConfig = byConfig.ToDictionary(x => x.Key, x => x.SelectMany(y => y));
            var averageByConfig = flattenedByConfig.Select(x => new BenchMarkResultsEntry(x.Key.Name, x.Value.Average()));
            averageFile.AddTotalAverages(averageByConfig);
            averageFile.Write(outputfolder + "Average.Results");
        }
    }

    public class RepoResultsFile : ResultsFile
    {
        public RepoResultsFile(IDictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>> perConfigEntry)
        {
            AddResultGroups(perConfigEntry.Select(x => new BenchmarkResultsGroup(x.Key.Name, x.Value)));
        }
    }

    public class AverageResultsFile : ResultsFile
    {
        public void AddRepoPerConfigAverages(string repoName,IEnumerable<BenchMarkResultsEntry> entries)
        {
            AddResultGroup(new BenchmarkResultsGroup(repoName,entries));
        }
        public void AddTotalAverages(IEnumerable<BenchMarkResultsEntry> entries)
        {
            AddResultGroup(new BenchmarkResultsGroup("Total averages", entries));
        }
    }

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

        public void WithResults(IEnumerable<BenchMarkResultsEntry> entries)
        {
            _lines.AddRange(entries.Select(x => x.ToString()));
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
