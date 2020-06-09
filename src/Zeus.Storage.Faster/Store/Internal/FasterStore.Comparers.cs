using System.Collections.Generic;
using FASTER.core;

namespace Zeus.Storage.Faster.Store.Internal
{
    internal partial class FasterStore<TKey, TValue>
    {
        public class KeyComparerAdapter : IFasterEqualityComparer<KeyHolder>, IEqualityComparer<TKey>
        {
            private readonly IFasterEqualityComparer<TKey> _keyComparer;


            public KeyComparerAdapter(IFasterEqualityComparer<TKey> keyComparer)
            {
                _keyComparer = keyComparer;
            }

            public bool Equals(ref KeyHolder k1, ref KeyHolder k2)
            {
                return _keyComparer.Equals(ref k1.Key, ref k2.Key);
            }


            public long GetHashCode64(ref KeyHolder k)
            {
                return _keyComparer.GetHashCode64(ref k.Key);
            }

            public bool Equals(TKey x, TKey y)
            {
                return _keyComparer.Equals(ref x, ref y);
            }

            public int GetHashCode(TKey obj)
            {
                return (int)_keyComparer.GetHashCode64(ref obj);
            }

        }
    }
}