using FASTER.core;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Zeus.Storage.Faster.Options;
using Zeus.Storage.Faster.Store.Internal;

namespace Zeus.Storage.Faster.Tests.Utils
{
    internal static class StoreFactory
    {
        public class StoreOptions : IOptions<FasterStoreOptions>
        {
            public StoreOptions(string directory)
            {
                Value = new FasterStoreOptions
                {
                    Directory = directory
                };
            }

            public FasterStoreOptions Value { get; }
        }

        internal static FasterStore<TKey, TValue> Create<TKey, TValue>(string directory, IFasterEqualityComparer<TKey> comparer)
        {
            return new FasterStore<TKey, TValue>(new StoreOptions(directory), comparer,
                new NullLogger<FasterStore<TKey, TValue>>());
        }
    }
}