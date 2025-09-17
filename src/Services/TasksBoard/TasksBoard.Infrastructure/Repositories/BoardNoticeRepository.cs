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
    public class BoardNoticeRepository(
        TasksBoardDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardNotice, BoardNoticeId>(context, loggerFactory), IBoardNoticeRepository
    {
        public async Task<IEnumerable<BoardNotice>> GetPaginatedByBoardIdAsync(
            BoardId boardId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.BoardId == boardId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAsync(
            AccountId userId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.AuthorId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAndBoardIdAsync(
            AccountId userId,
            BoardId boardId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.AuthorId == userId && notice.BoardId == boardId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(notice => notice.BoardId == boardId, cancellationToken);
        }

        public async Task<int> CountByUserIdAsync(
            AccountId userId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(notice => notice.AuthorId == userId, cancellationToken);
        }

        public async Task<int> CountByBoardIdAndUserIdAsync(
            BoardId boardId,
            AccountId userId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(notice => notice.BoardId == boardId && notice.AuthorId == userId, cancellationToken);
        }
    }
}
