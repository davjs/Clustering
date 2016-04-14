using System.Collections.Generic;

namespace Clustering.SolutionModel.Serializing
{
    public class FlatEntry
    {
        public string name;
        public IReadOnlyCollection<string> childData;
    }
}