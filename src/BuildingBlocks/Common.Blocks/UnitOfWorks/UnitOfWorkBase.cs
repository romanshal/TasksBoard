using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
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

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out object? value))
            {
                var repositoryInstance = new Repository<T>(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = value;
            }

            return (IRepository<T>)value;
        }

        public IOutboxEventRepository GetOutboxEventRepository()
        {
            var type = typeof(OutboxEvent);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<OutboxEvent>))
            {
                var repositoryInstance = new OutboxEventRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IOutboxEventRepository)value;
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
