using FASTER.core;

namespace Zeus.Storage.Faster.Tests.Utils
{
    internal static class Comparers
    {
        internal class IntComparer : IFasterEqualityComparer<int>
        {
            public static IntComparer Instance { get; } = new IntComparer();

            public long GetHashCode64(ref int k)
            {
                return Utility.GetHashCode(k);
            }

            public bool Equals(ref int k1, ref int k2)
            {
                return k1 == k2;
            }
        }
    }
}