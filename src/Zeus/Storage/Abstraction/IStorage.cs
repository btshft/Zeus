using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Storage.Abstraction
{
    public interface IStorage<TEntity> where TEntity : class, IIdentifiable
    {
        Task PersistAsync(TEntity entity, CancellationToken cancellation = default);

        Task<TEntity> GetAsync(Guid id, CancellationToken cancellation = default);

        Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);
    }
}