using Common.Blocks.Repositories;
using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardMemberRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardMember, BoardMemberId>(context, loggerFactory), IBoardMemberRepository
    {
        public async Task<IEnumerable<BoardMember>> GetByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(i => i.BoardMemberPermissions)
                .ThenInclude(i => i.BoardPermission)
                .Where(member => member.BoardId == boardId)
                .OrderByDescending(member => member.Board.OwnerId == member.AccountId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardMember>> GetPaginatedByBoardIdAsync(
            BoardId boardId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(i => i.BoardMemberPermissions)
                .ThenInclude(i => i.BoardPermission)
                .Where(member => member.BoardId == boardId)
                .OrderByDescending(member => member.Board.OwnerId == member.AccountId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        }

        public async Task<BoardMember?> GetByBoardIdAndAccountIdAsync(
            BoardId boardId,
            AccountId accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(i => i.BoardMemberPermissions)
                .ThenInclude(i => i.BoardPermission)
                .FirstOrDefaultAsync(member => member.BoardId == boardId && member.AccountId == accountId, cancellationToken);
        }

        public async Task<IEnumerable<BoardMember>> GetByAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(i => i.BoardMemberPermissions)
                .ThenInclude(i => i.BoardPermission)
                .Where(member => member.AccountId == accountId)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(member => member.BoardId == boardId, cancellationToken);
        }
    }
}
