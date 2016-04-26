using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Clustering.SolutionModel.Serializing
{
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