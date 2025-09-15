using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.UnitOfWorks
{
    internal class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<TasksBoardDbContext>(serviceProvider), IUnitOfWork
    {
        public IBoardNoticeRepository GetBoardNoticeRepository()
        {
            var type = typeof(BoardNotice);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardNotice, BoardNoticeId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardNoticeRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardNoticeRepository)value;
        }

        public IBoardRepository GetBoardRepository()
        {
            var type = typeof(Board);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<Board, BoardId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardRepository)value;
        }

        public IBoardMemberRepository GetBoardMemberRepository()
        {
            var type = typeof(BoardMember);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardMember, BoardMemberId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardMemberRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardMemberRepository)value;
        }

        public IBoardAccessRequestRepository GetBoardAccessRequestRepository()
        {
            var type = typeof(BoardAccessRequest);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardAccessRequest, BoardAccessId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardAccessRequestRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardAccessRequestRepository)value;
        }

        public IBoardInviteRequestRepository GetBoardInviteRequestRepository()
        {
            var type = typeof(BoardInviteRequest);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardInviteRequest, BoardInviteId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardInviteRequestRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardInviteRequestRepository)value;
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
    }
}
