using System.Collections.Generic;
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
                    new AlgorithmName(x.Key.Name), x.Value.Accuracy)));
            }

            return new ResultsTable(scores, runsPerAlg);
        }

        public IEnumerable<IBenchmarkConfig> Configs() => 
            _repoScores.Select(x => x.Value)
            .SelectMany(x => x.Keys).Distinct();
    }
}
