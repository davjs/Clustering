using System;
using System.Collections.Generic;
using System.Linq;
using Clustering.Benchmarking.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Benchmarking
{
    [TestClass]
    public class ResultsTableParser
    {
        [TestMethod]
        public void ParseResultsTable()
        {
            var table1 = ResultsTable.Parse(new []
            {
                "REPOSITORY ALG1 ALG2 ALG3 ALG4",
                "REPO1      0    1    2    3",
                "REPO2      4    5    6    7",
                "TOTAL-RUNS:2    2    2    2"
            });
            table1.Get(new ResultsTable.RepoName("REPO1"),
                new ResultsTable.AlgorithmName("ALG1")).Should().Be(0);
            table1.Get(new ResultsTable.RepoName("REPO2"),
                new ResultsTable.AlgorithmName("ALG4")).Should().Be(7);
        }
    }
}
