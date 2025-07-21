using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<Board>(context, loggerFactory), IBoardRepository
    {
        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(board => board.BoardMembers.Any(member => member.AccountId == userId))
                .OrderBy(e => e.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(Guid userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(board => board.BoardMembers.Any(member => member.AccountId == userId) && board.Name.ToLower().StartsWith(query.Trim().ToLower()))
                .OrderBy(e => e.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
            .AsNoTracking()
            .Where(board => board.Public)
            .OrderBy(e => e.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasAccessAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(board => board.Id == boardId && board.BoardMembers.Any(member => member.AccountId == userId), cancellationToken);
        }

        public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(board => board.BoardMembers.Any(member => member.AccountId == userId), cancellationToken);
        }

        public async Task<int> CountByUserIdAndQueryAsync(Guid userId, string query, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(board => board.BoardMembers.Any(member => member.AccountId == userId) && board.Name.ToLower().StartsWith(query.Trim().ToLower()), cancellationToken);
        }

        public async Task<int> CountPublicAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.AsNoTracking()
                .CountAsync(board => board.Public, cancellationToken);
        }
    }
}
