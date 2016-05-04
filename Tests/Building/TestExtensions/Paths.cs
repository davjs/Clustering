using System;

namespace Tests.Building.TestExtensions
{
    public static class Paths
    {
        public static readonly string SolutionFolder =
            AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\";
        public static readonly string ThisSolution = SolutionFolder + "Clustering.sln";
    }
}