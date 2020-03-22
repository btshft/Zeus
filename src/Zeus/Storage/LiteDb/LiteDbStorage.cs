using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Zeus.Storage.Abstraction;

namespace Zeus.Storage.LiteDb
{
    public class LiteDbStorage<T> : IStorage<T> where T : class, IIdentifiable
    {
        private readonly LiteDbProvider _provider;

        protected LiteDatabase Database => _provider.GetDatabase();

        protected ILiteCollection<T> Collection => Database.GetCollection<T>();

        public LiteDbStorage(LiteDbProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public Task PersistAsync(T entity, CancellationToken cancellation = default)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));

            Collection.Upsert(entity);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<T> GetAsync(Guid id, CancellationToken cancellation = default)
        {
            return Task.FromResult(Collection.FindById(id));
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            IReadOnlyCollection<T> results = Collection.Find(predicate).ToArray();
            return Task.FromResult(results);
        }

        public Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            var affectedRows = Collection.DeleteMany(predicate);
            return Task.FromResult(affectedRows > 0);
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            return Task.FromResult(Collection.Exists(predicate));
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            var result = Collection.FindOne(predicate);
            return Task.FromResult(result);
        }
    }
}
