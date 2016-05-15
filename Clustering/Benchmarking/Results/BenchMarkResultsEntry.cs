using System.Collections.Generic;
using System.Linq;

namespace Clustering.Benchmarking.Results
{
    public class BenchMarkResultsEntry
    {
        public readonly string _name;
        public readonly BenchMarkResult _result;
        public BenchMarkResultsEntry(string name, BenchMarkResult result)
        {
            _name = name;
            _result = result;
        }
        

        public override string ToString() => $"{_name} : {_result}";
    }

    public static class BenchMarkResultsExtensions
    {
        public static BenchMarkResult Average(this IReadOnlyCollection<BenchMarkResult> scores)
        {
            return scores.Any(x => x.Succeeded) ?
                new BenchMarkResult(scores.Where(x => x.Succeeded).Select(x => x._accuracy).Average()) :
                new BenchMarkResult(scores.First()._errorMessage);
        }

        public static BenchMarkResultsEntry Average(this BenchmarkResultsGroup group) =>
            new BenchMarkResultsEntry(group.Header, group.Entries.ToList().Average());

        public static BenchMarkResult Average(this IReadOnlyCollection<BenchMarkResultsEntry> scores)
        {
            var sucessScores = scores.Where(x => x._result.Succeeded).ToList();
            if (!sucessScores.Any())
                return scores.First()._result;
            return new BenchMarkResult(sucessScores.Select(x => x._result._accuracy).Average());
        }
    }
}