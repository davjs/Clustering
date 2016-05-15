using System.Collections.Generic;
using System.Linq;

namespace Clustering.Benchmarking.Results
{
    public class BenchMarkResultsEntry
    {
        public readonly string Name;
        public readonly BenchMarkResult Result;
        public BenchMarkResultsEntry(string name, BenchMarkResult result)
        {
            Name = name;
            Result = result;
        }
        

        public override string ToString() => $"{Name} : {Result}";
    }

    public static class BenchMarkResultsExtensions
    {
        public static BenchMarkResult Average(this IReadOnlyCollection<BenchMarkResult> scores)
        {
            return scores.Any(x => x.Succeeded) ?
                new BenchMarkResult(scores.Where(x => x.Succeeded).Select(x => x.Accuracy).Average()) :
                new BenchMarkResult(scores.First().ErrorMessage);
        }

        //public static BenchMarkResultsEntry Average(this BenchmarkResultsGroup group) =>
        //    new BenchMarkResultsEntry(group.Header, group.Entries.ToList().Average());

        public static BenchMarkResult Average(this IReadOnlyCollection<BenchMarkResultsEntry> scores)
        {
            var sucessScores = scores.Where(x => x.Result.Succeeded).ToList();
            if (!sucessScores.Any())
                return scores.First().Result;
            return new BenchMarkResult(sucessScores.Select(x => x.Result.Accuracy).Average());
        }
    }
}