using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Infrastructure.Data.Contexts;
using TasksBoard.Infrastructure.Repositories;

namespace TasksBoard.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : IUnitOfWork
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

        public IBoardNoticeRepository GetBoardNoticeRepository()
        {
            var type = typeof(BoardNotice);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardNotice>))
            {
                var repositoryInstance = new BoardNoticeRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardNoticeRepository)value;
        }

        public IBoardRepository GetBoardRepository()
        {
            var type = typeof(Board);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<Board>))
            {
                var repositoryInstance = new BoardRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardRepository)value;
        }

        public IBoardMemberRepository GetBoardMemberRepository()
        {
            var type = typeof(BoardMember);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardMember>))
            {
                var repositoryInstance = new BoardMemberRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardMemberRepository)value;
        }

        public IBoardAccessRequestRepository GetBoardAccessRequestRepository()
        {
            var type = typeof(BoardAccessRequest);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardAccessRequest>))
            {
                var repositoryInstance = new BoardAccessRequestRepsitory(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardAccessRequestRepository)value;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //private R GetRepository<R, E>() where R : Repository<E>, new() where E : BaseEntity
        //{
        //    var type = typeof(E);

        //    if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<E>))
        //    {
        //        var repositoryInstance = Repository<E>.CreateInstanse<E>(_context, _loggerFactory);

        //        value = repositoryInstance;

        //        _repositories[type] = repositoryInstance;
        //    }

        //    return (R)value;
        //}
    }
}
