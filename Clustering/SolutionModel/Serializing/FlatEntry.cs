using System.Collections.Generic;

namespace Clustering.SolutionModel.Serializing
{
    public class FlatEntry
    {
        public string path;
        public IReadOnlyCollection<string> childData;
    }
}