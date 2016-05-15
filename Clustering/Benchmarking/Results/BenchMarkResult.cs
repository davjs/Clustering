namespace Clustering.Benchmarking.Results
{
    public class BenchMarkResult
    {
        public readonly string ErrorMessage;
        public readonly double Accuracy;
        public bool Succeeded => ErrorMessage == null;

        public BenchMarkResult(double accuracy)
        {
            Accuracy = accuracy;
        }

        public BenchMarkResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return ErrorMessage ?? Accuracy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}