using System;

namespace BenchMarking
{
    public class NotEnoughLeafNamespacesException : Exception
    {
        public NotEnoughLeafNamespacesException() : base("NOT_ENOUGH_LEAF_NAMESPACES")
        {

        }
    }
}