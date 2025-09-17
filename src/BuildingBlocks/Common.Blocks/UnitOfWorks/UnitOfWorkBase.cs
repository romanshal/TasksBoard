using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Common.Blocks.UnitOfWorks
{
    public class UnitOfWorkBase<TContext> : IUnitOfWorkBase where TContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly AsyncServiceScope _scope;
        private readonly DbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = [];

        public UnitOfWorkBase(IServiceProvider serviceProvider)
        {
            this._scope = serviceProvider.CreateAsyncScope();
            this._loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            this._context = _scope.ServiceProvider.GetRequiredService<TContext>();
        }

        public IRepository<T, TId> GetRepository<T, TId>() where T : class, IEntity<TId> where TId : ValueObject
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out object? value))
            {
                //var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IRepository<T, TId>>();
                var repositoryInstance = new Repository<T, TId>(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = value;
            }

            return (IRepository<T, TId>)value;
        }

        public TRepository GetCustomRepository<TRepository>() where TRepository : class
        {
            var type = typeof(TRepository);
            if (!_repositories.TryGetValue(type, out var value))
            {
                value = Activator.CreateInstance(typeof(TRepository), _context, _loggerFactory) ?? throw new InvalidOperationException($"Cannot create instance of {type.Name}");
                _repositories[type] = value;
            }

            return (TRepository)value;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task TransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken = default)
        {
            var hasActic = _context.Database.CurrentTransaction is not null;

            if (hasActic)
            {
                await action(cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await action(cancellationToken);
            await SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

        public async Task<TResult> TransactionAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken cancellationToken = default)
        {
            var hasActive = _context.Database.CurrentTransaction is not null;

            if (hasActive)
            {
                return await action(cancellationToken);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var result = await action(cancellationToken);
            await SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }

        public async virtual ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable scopeAsyncDisposable) await scopeAsyncDisposable.DisposeAsync();
            else _scope.Dispose();

            if(_context is IAsyncDisposable contextAsyncDisposable) await contextAsyncDisposable.DisposeAsync();
            else _context.Dispose();
        }

        public virtual void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        protected TRepository Repository<TEntity, TId, TRepository>()
            where TEntity : class, IEntity<TId>
            where TId : ValueObject
            where TRepository : notnull, IRepository<TEntity, TId>
        {
            var type = typeof(TEntity);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<TEntity, TId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<TRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (TRepository)value;
        }
    }
}
