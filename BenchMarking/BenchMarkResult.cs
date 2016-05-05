using System;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;

namespace BenchMarking
{
    public class BenchMarkResult
    {
        public readonly string ProjectName;
        private readonly Exception _exception;
        public readonly double Accuracy;

        public BenchMarkResult(string projectName, double accuracy)
        {
            Accuracy = accuracy;
            ProjectName = projectName;
        }

        public BenchMarkResult(string projectName, Exception exception)
        {
            ProjectName = projectName;
            _exception = exception;
        }

        public override string ToString()
        {
            var rightHandSide = _exception?.Message
                    ?? Accuracy.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return $"{ProjectName} : {rightHandSide}";
        }
    }
}
