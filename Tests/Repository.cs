using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Repository
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Solution { get; set; }

        public Repository(string name, string owner, string solution)
        {
            Name = name;
            Owner = owner;
            Solution = solution;
        }
    }
}
