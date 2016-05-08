namespace Clustering.Benchmarking
{
    public class BenchMarkResult
    {
        public readonly string _errorMessage;
        public readonly double _accuracy;
        public bool Succeeded => _errorMessage == null;

        public BenchMarkResult(double accuracy)
        {
            _accuracy = accuracy;
        }

        public BenchMarkResult(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public override string ToString()
        {
            return _errorMessage ?? _accuracy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}