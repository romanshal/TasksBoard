using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(TasksBoardDbContext context, ILoggerFactory loggerFactory) : IUnitOfWork
    {
        private readonly TasksBoardDbContext _context = context;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly Dictionary<Type, object> _repositories = [];

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
