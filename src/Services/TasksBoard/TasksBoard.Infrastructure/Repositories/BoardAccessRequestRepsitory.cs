using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardAccessRequestRepsitory(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardAccessRequest, BoardAccessId>(context, loggerFactory), IBoardAccessRequestRepository
    {
        public async Task<IEnumerable<BoardAccessRequest>> GetByBoardIdAsync(BoardId boardId, CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .Where(request => request.BoardId == boardId && request.Status == (int)BoardAccessRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<BoardAccessRequest?> GetByBoardIdAndAccountId(BoardId boardId, Guid accountId, CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .OrderByDescending(request => request.CreatedAt)
                .FirstOrDefaultAsync(request => request.BoardId == boardId && request.AccountId == accountId, cancellationToken);
        }

        public async Task<IEnumerable<BoardAccessRequest>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(request => request.AccountId == accountId && request.Status == (int)BoardAccessRequestStatuses.Pending)
                .ToListAsync(cancellationToken);
        }
    }
}
