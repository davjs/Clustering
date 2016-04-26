using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Clustering.Hierarchichal;
using Clustering.SolutionModel.Nodes;
using MoreLinq;

namespace Clustering.Hierarchichal.CuttingAlgorithms
{
    public interface ICuttingAlgorithm
    {
        IEnumerable<Node> Cut(ISet<Node> tree);
    }
}
