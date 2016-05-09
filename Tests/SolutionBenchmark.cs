using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Clustering;
using Clustering.Benchmarking;
using Clustering.Benchmarking.Results;
using Clustering.SolutionModel.Serializing;
using Tests.Building.TestExtensions;

namespace Tests
{
    public static class SolutionBenchmark
    {
        private static string ParsedRepoLocation(Repository repo) =>
            $@"{LocalPathConfig.ParsedDataLocation}\{repo.Owner}\{repo.Name}\";

        private static string RepositoryLocation(Repository repo) =>
            $@"{LocalPathConfig.RepoLocations}\{repo.Name}\{repo.Solution}";

        public static ResultsContainer RunAllInFolder(IReadOnlyCollection<IBenchmarkConfig> benchMarkConfigs,
            IList<Repository> repos)
        {

            var repoScores = new Dictionary<Repository, Dictionary<IBenchmarkConfig, List<BenchMarkResultsEntry>>>();
            
            foreach (var repository in repos.ToList())
            {
                var dataFolder = ParsedRepoLocation(repository);
                var projectGraphsInFolder = BenchMark.GetProjectGraphsInFolder(dataFolder).ToList();

                var configEntries = new Dictionary<IBenchmarkConfig,List<BenchMarkResultsEntry>>();
                foreach (var project in projectGraphsInFolder)
                {
                    var leafNamespaces = BenchMark.GetLeafNamespaces(project);
                    
                    foreach (var config in benchMarkConfigs)
                    {
                        var results = Enumerable.Range(0, 10).Select(x => BenchMark.Run(config, leafNamespaces)).ToList();
                        var benchMarkResult = new BenchMarkResultsEntry(project.Name, results.Average());

                        if(!configEntries.ContainsKey(config))
                            configEntries[config] = new List<BenchMarkResultsEntry>();
                        configEntries[config].Add(benchMarkResult);
                    }
                }
                repoScores.Add(repository, configEntries);
            }

            return new ResultsContainer(repoScores);
        }

        public static void Prepare(Repository repo)
        {
            BenchMark.Prepare(RepositoryLocation(repo), ParsedRepoLocation(repo));
        }
    }
}
