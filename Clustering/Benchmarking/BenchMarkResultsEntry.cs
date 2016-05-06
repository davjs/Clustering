namespace Clustering.Benchmarking
{
    public class BenchMarkResultsEntry
    {
        private readonly string _projectName;
        private readonly BenchMarkResult _result;
        public BenchMarkResultsEntry(string projectName, BenchMarkResult result)
        {
            _projectName = projectName;
            _result = result;
        }

        public override string ToString() => $"{_projectName} : {_result}";
    }
}