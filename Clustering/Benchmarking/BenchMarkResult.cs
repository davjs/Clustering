namespace Clustering.Benchmarking
{
    public class BenchMarkResult
    {
        private readonly string _errorMessage;
        private readonly double _accuracy;

        public BenchMarkResult(double accuracy)
        {
            _accuracy = accuracy;
        }

        public BenchMarkResult(string errorMessage)
        {
            this._errorMessage = errorMessage;
        }

        public override string ToString()
        {
            return _errorMessage ?? _accuracy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}