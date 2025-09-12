using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Blocks.UnitOfWorks
{
    public class UnitOfWorkBase(
        DbContext context,
        ILoggerFactory loggerFactory) : IUnitOfWorkBase
    {
        private readonly DbContext _context = context;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        protected readonly Dictionary<Type, object> _repositories = [];

        public IRepository<T, TId> GetRepository<T, TId>() where T : class, IEntity<TId> where TId : ValueObject
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out object? value))
            {
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
