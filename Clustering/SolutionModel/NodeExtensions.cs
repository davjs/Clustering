using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.SolutionModel
{
    public static class NodeExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> list,T item) =>
            list.Except(new HashSet<T>{item});
    }
}
