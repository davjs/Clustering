using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clustering.Benchmarking.Results
{
    public class PerSolutionResultsContainer
    {
        private readonly Dictionary<Repository, Dictionary<IBenchmarkConfig, BenchMarkResult>> _repoScores;
        private readonly int _rerunsPerConfig;

        public PerSolutionResultsContainer(Dictionary<Repository, Dictionary<IBenchmarkConfig, BenchMarkResult>> repoScores, int rerunsPerConfig)
        {
            _repoScores = repoScores;
            _rerunsPerConfig = rerunsPerConfig;
        }

        public void WriteToFolder(string outputfolder)
        {
            outputfolder = outputfolder + "complete\\";
            Directory.CreateDirectory(outputfolder);
            // Per repo file
            foreach (var repositoryBenchMarks in _repoScores)
            {
                var repository = repositoryBenchMarks.Key;
                var benchMarkByConfig = repositoryBenchMarks.Value;
                new ResultsFile().WithResults(benchMarkByConfig.Select(x => new BenchMarkResultsEntry(x.Key.Name,x.Value)))
                    .Write(outputfolder + $"{repository.Name}.Results");
            }

            // Average file
            var averageFile = new AverageResultsFile();

            var repoLess = _repoScores.Select(x => x.Value);

            var configs = repoLess.SelectMany(x => x.Keys).Distinct();

            var averageByConfig = from config in configs
                let average = repoLess.Select(x => x[config]).ToList().Average()
                select new BenchMarkResultsEntry(config.Name,average);
            
            averageFile.AddRunsPerConfig(_rerunsPerConfig);
            averageFile.AddTotalAverages(averageByConfig);
            averageFile.Write(outputfolder + "Average.Results");
        }
    }

    public class PerProjectResultsContainer
    {
        private readonly Dictionary<Repository, Dictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>>> _repoScores;

        public PerProjectResultsContainer(
            Dictionary<Repository, Dictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>>> repoScores)
        {
            _repoScores = repoScores;
        }

        public void WriteToFolder(string outputfolder)
        {
            Directory.CreateDirectory(outputfolder);
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
        public void AddRunsPerConfig(int runsPerConfig)
        {
            AddEntry(new BenchMarkResultsEntry("TOTAL RUNS", new BenchMarkResult(runsPerConfig)));
        }

        public void AddRepoPerConfigAverages(string repoName,IEnumerable<BenchMarkResultsEntry> entries)
        {
            AddResultGroup(new BenchmarkResultsGroup(repoName,entries));
        }
        public void AddTotalAverages(IEnumerable<BenchMarkResultsEntry> entries)
        {
            AddResultGroup(new BenchmarkResultsGroup("Total averages", entries));
        }
    }
}
