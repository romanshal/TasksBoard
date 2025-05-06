using Chat.Domain.Entities;
using Chat.Domain.Interfaces.Repositories;
using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Infrastructure.Data.Contexts;
using Chat.Infrastructure.Repositories;
using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.Extensions.Logging;

namespace Chat.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        ChatDbContext context,
        ILoggerFactory loggerFactory) : IUnitOfWork
    {
        private readonly ChatDbContext _context = context;
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

        public IBoardMessageRepository GetBoardMessagesRepository()
        {
            var type = typeof(BoardMessage);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardMessage>))
            {
                var repositoryInstance = new BoardMessageRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardMessageRepository)value;
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
