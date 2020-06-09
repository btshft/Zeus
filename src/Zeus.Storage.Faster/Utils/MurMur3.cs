using System;
using System.Security.Cryptography;
using System.Text;
using Murmur;

namespace Zeus.Storage.Faster.Utils
{
    public static class MurMur3
    {
        private static readonly Lazy<HashAlgorithm> AlgorithmProvider = new Lazy<HashAlgorithm>(
            () => MurmurHash.Create128());

        public static byte[] Hash(byte[] input)
        {
            var algorithm = AlgorithmProvider.Value;
            return algorithm.ComputeHash(input);
        }

        public static byte[] Hash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Hash(bytes);
        }
    }
}
