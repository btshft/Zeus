using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Abstraction;

namespace Zeus.Storage.Extensions
{
    public static class StorageExtensions
    {
        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IStorage<TEntity> storage,
            CancellationToken cancellation = default)
            where TEntity : class, IIdentifiable
        {
            return storage.FirstOrDefaultAsync(s => true, cancellation);
        }
    }
}
