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
    public class BoardRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<Board, BoardId>(context, loggerFactory), IBoardRepository
    {
        public async Task<Board?> GetAsync(
            BoardId id,
            bool noTracking = true, 
            bool include = true, 
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(board => board.Id == id);

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            if (include)
            {
                query = query
                    .Include(i => i.BoardMembers)
                    .ThenInclude(i => i.BoardMemberPermissions)
                    .ThenInclude(i => i.BoardPermission)
                    .Include(i => i.BoardImage)
                    .Include(i => i.BoardTags)
                    .Include(i => i.BoardAccessRequests)
                    .Include(i => i.BoardInviteRequests);
            }

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(AccountId userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(i => i.BoardMembers)
                .Include(i => i.BoardImage)
                .Include(i => i.BoardTags)
                .Where(board => board.BoardMembers.Any(member => member.AccountId == userId))
                .OrderBy(e => e.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(AccountId userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(i => i.BoardMembers)
                .Include(i => i.BoardImage)
                .Include(i => i.BoardTags)
                .Where(board => board.BoardMembers.Any(member => member.AccountId == userId) && board.Name.StartsWith(query.Trim(), StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(e => e.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
            .AsNoTracking()
            .Include(i => i.BoardMembers)
            .Include(i => i.BoardImage)
            .Include(i => i.BoardTags)
            .Where(board => board.Public)
            .OrderBy(e => e.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasAccessAsync(BoardId boardId, AccountId userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(board => board.Id == boardId && board.BoardMembers.Any(member => member.AccountId == userId), cancellationToken);
        }

        public async Task<int> CountByUserIdAsync(AccountId userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(board => board.BoardMembers.Any(member => member.AccountId == userId), cancellationToken);
        }

        public async Task<int> CountByUserIdAndQueryAsync(AccountId userId, string query, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(board => board.BoardMembers.Any(member => member.AccountId == userId) && board.Name.StartsWith(query.Trim(), StringComparison.CurrentCultureIgnoreCase), cancellationToken);
        }

        public async Task<int> CountPublicAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.AsNoTracking()
                .CountAsync(board => board.Public, cancellationToken);
        }
    }
}
