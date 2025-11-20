using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;

namespace Common.Blocks.Interfaces.UnitOfWorks
{
    public interface IUnitOfWorkBase : IAsyncDisposable, IDisposable
    {
        IRepository<T, TId> GetRepository<T, TId>() where T : class, IEntity<TId> where TId : ValueObject;

        TRepository GetRepository<TEntity, TId, TRepository>()
            where TEntity : class, IEntity<TId>
            where TId : ValueObject
            where TRepository : notnull, IRepository<TEntity, TId>;

        TRepository GetCustomRepository<TRepository>() where TRepository : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task TransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
        Task<TResult> TransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default);
    }
}
