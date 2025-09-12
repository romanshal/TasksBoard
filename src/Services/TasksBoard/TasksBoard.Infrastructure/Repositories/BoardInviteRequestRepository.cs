using Common.Blocks.Constants;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardInviteRequestRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardInviteRequest, BoardInviteId>(context, loggerFactory), IBoardInviteRequestRepository
    {
        public async Task<BoardInviteRequest?> GetByBoardIdAndToAccountIdAsync(
            BoardId boardId,
            Guid toAccountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(invite => invite.BoardId == boardId && invite.ToAccountId == toAccountId, cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .Where(request => request.BoardId == boardId && request.Status == (int)BoardInviteRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByToAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(request => request.ToAccountId == accountId && request.Status == (int)BoardInviteRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardInviteRequest>> GetByFromAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(request => request.FromAccountId == accountId)
                .ToListAsync(cancellationToken);
        }
    }
}
