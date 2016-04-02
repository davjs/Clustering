using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;

namespace Clustering
{
    class ClusterBenchmarker
    {
        public static PrecisionRecal PrecisionRecal(IEnumerable<Node> test, IEnumerable<Node> thruth)
        {
            throw new NotImplementedException();
        }
    }

    public struct PrecisionRecal
    {
        public decimal Precision;
        public decimal Recall;
    }
}
