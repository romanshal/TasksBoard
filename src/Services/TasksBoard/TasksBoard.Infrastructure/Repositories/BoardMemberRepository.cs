using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardMemberRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardMember>(context, loggerFactory), IBoardMemberRepository
    {
        public async Task<IEnumerable<BoardMember>> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(member => member.BoardId == boardId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardMember>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(member => member.BoardId == boardId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);

        }

        public async Task<BoardMember?> GetByBoardIdAndUserIdAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(member => member.BoardId == boardId && member.AccountId == userId, cancellationToken);
        }

        public async Task<int> CountByBoardIdAsync(Guid boardId, CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(member => member.BoardId == boardId, cancellationToken);
        }
    }
}
