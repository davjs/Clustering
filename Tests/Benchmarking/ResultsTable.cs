using System;
using System.Collections.Generic;
using System.Linq;
using Clustering.Benchmarking.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Building.TestExtensions;
using System.IO;

namespace Tests.Benchmarking
{
    [TestClass]
    public class ResultsTableTests
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
            table1.ResultFor(new RepoName("REPO1"),
                new AlgorithmName("ALG1")).Should().Be(0);
            table1.ResultFor(new RepoName("REPO2"),
                new AlgorithmName("ALG4")).Should().Be(7);
        }

        [TestMethod]
        public void MergeTables()
        {
            var table1 = ResultsTable.Parse(new[]
            {
                "REPOSITORY ALG1 ALG2 ALG3",
                "REPO1      0    1    2",
                "REPO2      4    5    6",
                "TOTAL-RUNS 2    1    2"
            });

            var table2 = ResultsTable.Parse(new[]
            {
                "REPOSITORY ALG1 ALG2 ALG3 ALG4",
                "REPO1      0    1    2    3",
                "REPO2      4    10   6    7",
                "TOTAL-RUNS 2    2    2    2"
            });

            var newTable = table1.Combine(table2);

            // ReSharper disable once PossibleLossOfFraction
            newTable.ResultFor(new RepoName("REPO2"), new AlgorithmName("ALG2")).
                Should().Be((5 + 10*2)/3.0);
        }

        [TestMethod]
        public void ParsesTotalRuns()
        {
            var table = ResultsTable.Parse(new[]
            {
                "REPOSITORY ALG1 ALG2 ALG3",
                "REPO1      0    1    2",
                "REPO2      4    5    6",
                "TOTAL-RUNS 2    1    2"
            });

            table.TotalRuns[new AlgorithmName("ALG1")].Should().Be(2);
            table.TotalRuns[new AlgorithmName("ALG2")].Should().Be(1);
            table.TotalRuns[new AlgorithmName("ALG3")].Should().Be(2);
        }

        [TestMethod]
        public void FormattingTest()
        {
            var originalLines = new[]
            {
                "Repo/Algorithm ALG1 ALG2 ALG3",
                "REPO1          0    1    2",
                "REPO2          4    5    6",
                "TOTAL-RUNS     2    1    2"
            };
            var table = ResultsTable.Parse(originalLines);
            var formattedLines = table.FormattedLines();
            formattedLines.ShouldBeEquivalentTo(originalLines);
        }

        [TestMethod]
        public void ParseTableAtPath()
        {
            var table = ResultsTable.Parse(Paths.SolutionFolder + "BenchMarkResults\\Complete.results");
        }

        [TestMethod]
        public void ConvertTableToLatex()
        {
            ResultsTable.Parse(Paths.SolutionFolder + "BenchMarkResults\\Complete.results")
                .WriteLatex(Paths.SolutionFolder + "latex.txt");
        }

        [TestMethod]
        public void CombineTables()
        {
            var t1 = ResultsTable.Parse(Paths.SolutionFolder + "BenchMarkResults\\1.results)");
            var t2 = ResultsTable.Parse(Paths.SolutionFolder + "BenchMarkResults\\2.results)");
            var combined = t1.Combine(t2);
            File.WriteAllLines(Paths.SolutionFolder + "BenchMarkResults\\Combined.results", combined.FormattedLines());
        }
    }
}
