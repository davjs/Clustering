using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Clustering;
using Clustering.Benchmarking;
using Clustering.Benchmarking.Results;
using Clustering.SolutionModel.Nodes;
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

        public static PerProjectResultsContainer RunAllInFolder(IReadOnlyCollection<IBenchmarkConfig> benchMarkConfigs,
            IList<Repository> repos, int rerunsPerConfig)
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
                        var results = Enumerable.Range(0, rerunsPerConfig).Select(x => BenchMark.Run(config, leafNamespaces)).ToList();
                        var benchMarkResult = new BenchMarkResultsEntry(project.Name, results.Average());

                        if(!configEntries.ContainsKey(config))
                            configEntries[config] = new List<BenchMarkResultsEntry>();
                        configEntries[config].Add(benchMarkResult);
                    }
                }
                repoScores.Add(repository, configEntries);
            }

            return new PerProjectResultsContainer(repoScores);
        }

        public static PerSolutionResultsContainer RunAllCompletesInFolder(IReadOnlyCollection<IBenchmarkConfig> benchMarkConfigs,
            IList<Repository> repos,int rerunsPerConfig)
        {

            var repoScores = new Dictionary<Repository, Dictionary<IBenchmarkConfig,BenchMarkResult>>();

            foreach (var repository in repos.ToList())
            {
                var dataFolder = ParsedRepoLocation(repository);
                var graph = BenchMark.GetCompleteTreeWithDependencies(dataFolder);

                var leafNamespaces = BenchMark.RootNamespaces(graph);

                //CODE FOR REMOVING DEPENDENCIES
                /*var newEdges = (from edge in leafNamespaces.Edges
                    let newDeps = new HashSet<Node>(edge.Take((int)(edge.Count()*0.25)))
                    select new {edge.Key, newDeps })
                    .ToDictionary(x => x.Key, x => x.newDeps)
                    .SelectMany(p => p.Value
                                         .Select(x => new { p.Key, Value = x }))
                       .ToLookup(pair => pair.Key, pair => pair.Value);*/
                //leafNamespaces = new NonNestedClusterGraph(leafNamespaces.Clusters, newEdges);

                leafNamespaces = new NonNestedClusterGraph(leafNamespaces.Clusters, leafNamespaces.Edges);


                var configEntries = new Dictionary<IBenchmarkConfig, BenchMarkResult>();
                foreach (var config in benchMarkConfigs)
                {
                    var tasks = Enumerable.Range(0, rerunsPerConfig)
                        .Select(x => Task.Run(() => BenchMark.Run(config, leafNamespaces))).ToList();

                    var results = Task.WhenAll(tasks).Result;

                    BenchMarkResult average = results.Average();
                    var unknown = new {config, average};
                    configEntries.Add(unknown.config, unknown.average);
                }

                repoScores.Add(repository, configEntries);
            }

            return new PerSolutionResultsContainer(repoScores, rerunsPerConfig);
        }

        public static void Prepare(Repository repo)
        {
            BenchMark.Prepare(RepositoryLocation(repo), ParsedRepoLocation(repo));
        }
    }
}
