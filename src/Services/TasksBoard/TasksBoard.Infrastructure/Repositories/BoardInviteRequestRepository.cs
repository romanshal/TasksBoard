using Common.Blocks.Constants;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardInviteRequestRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardInviteRequest>(context, loggerFactory), IBoardInviteRequestRepository
    {
        public async Task<BoardInviteRequest?> GetByBoardIdAndToAccountIdAsync(Guid boardId, Guid toAccountId, CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(invite => invite.BoardId == boardId && invite.ToAccountId == toAccountId, cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .Where(request => request.BoardId == boardId && request.Status == (int)BoardInviteRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByToAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(request => request.ToAccountId == accountId && request.Status == (int)BoardInviteRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByFromAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(request => request.FromAccountId == accountId)
                .ToListAsync(cancellationToken);
        }
    }
}
