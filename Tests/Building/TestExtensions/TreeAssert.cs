using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.String;

namespace Tests.Building.TestExtensions
{
    class TreeAssert
    {
        public static void TreeEquals(ISet<Node> expected, ISet<Node> actual)
        {
            if (expected.Count != actual.Count)
                throw new AssertFailedException($"Expected {expected} to have {expected.Count} childs got {actual.Count}\n" +
                                                "Expected:\n" +
                                                $" {expected}\n " +
                                                "--------------------\nGot:\n" +
                                                $" {actual}");
            foreach (var child in expected)
            {
                var foundChild = false;
                if (IsNullOrEmpty(child.Name))
                {
                    if (actual.Any(x => CompareByChildren(child, x)))
                        foundChild = true;
                }
                else
                {
                    if (actual.WithName(child.Name) != null)
                        foundChild = true;
                }
                if (!foundChild)
                    throw new AssertFailedException($"{expected} did not have child {child} got {actual}\n" +
                                                    "Expected:\n" +
                                                    $" {expected}\n " +
                                                    "--------------------\nGot:\n" +
                                                    $" {actual}");
            }
        }

        private static bool CompareByChildren(Node expected, Node actual)
        {
            foreach (var child in expected.Children)
            {
                if (IsNullOrEmpty(child.Name))
                {
                    if (!actual.Children.Any(x => CompareByChildren(child, x)))
                        return false;
                }
                else
                {
                    if (actual.Children.WithName(child.Name) == null)
                        return false;
                }
            }
            return true;
        }
    }
}
