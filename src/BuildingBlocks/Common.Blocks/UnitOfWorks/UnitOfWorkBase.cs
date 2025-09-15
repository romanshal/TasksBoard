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
        protected readonly AsyncServiceScope _scope;
        protected readonly DbContext _context;
        protected readonly ConcurrentDictionary<Type, object> _repositories = [];

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

        public async virtual ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable scopeAsyncDisposable)
            {
                await scopeAsyncDisposable.DisposeAsync();
                await _context.DisposeAsync();
            }
            else
            {
                _scope.Dispose();
                _context.Dispose();
            }
        }

        public virtual void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
