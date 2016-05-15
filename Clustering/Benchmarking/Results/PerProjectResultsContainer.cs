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

        public ResultsTable ToResultsTable()
        {
            // The same for every algorithm, only the combination of two results file changes this
            var runsPerAlg = Configs()
                .ToDictionary(config => new AlgorithmName(config.Name), config => _rerunsPerConfig);

            var scores = new List<Score>();
            
            foreach (var repoScore in _repoScores)
            {
                var repo = repoScore.Key;
                scores.AddRange(repoScore.Value.Select(x => new Score(new RepoName(repo.Name),
                    new AlgorithmName(x.Key.Name), x.Value._accuracy)));
            }

            return new ResultsTable(scores, runsPerAlg);
        }

        public IEnumerable<IBenchmarkConfig> Configs() => 
            _repoScores.Select(x => x.Value)
            .SelectMany(x => x.Keys).Distinct();

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
            var configs = Configs();

            var averageByConfig = from config in configs
                let average = repoLess.Select(x => x[config]).ToList().Average()
                select new BenchMarkResultsEntry(config.Name,average);
            
            averageFile.AddRunsPerConfig(_rerunsPerConfig);
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
