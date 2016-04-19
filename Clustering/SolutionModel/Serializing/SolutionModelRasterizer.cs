using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;

namespace Clustering.SolutionModel.Serializing
{
    public static class SolutionModelRasterizer
    {
        public static void Write(SolutionNode solutionModel, string path)
        {
            foreach (var projectNode in solutionModel.Projects())
            {
                var dependencies = DependencyResolver.GetDependencies(projectNode.Classes());
                var encodedString = projectNode.Children.EncodeGraph(
                    n => n.Children,
                    n => n.Name,
                    n => dependencies[n]);
                var sha1 = SHA1Util.SHA1HashStringForUTF8String(encodedString);
                var finalString = sha1 + "\n" + encodedString;
                File.WriteAllText(path + projectNode.Name, finalString);
            }
        }
    }
    
    public static class SHA1Util
    {
        /// <summary>
        /// Compute hash for string encoded as UTF8
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <returns>40-character hex string</returns>
        public static string SHA1HashStringForUTF8String(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(bytes);
                return HexStringFromBytes(hashBytes);
            }
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        private static string HexStringFromBytes(IEnumerable<byte> bytes) =>
            string.Concat(bytes.Select(b => b.ToString("x2")));

    }
}
