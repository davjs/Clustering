using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Repository
    {
        public string Name { get; }
        public string Owner { get; }
        public string Solution { get; }

        public Repository(string name, string owner, string solution)
        {
            Name = name;
            Owner = owner;
            Solution = solution;
        }
    }
}
