using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardNoticeRepository(
        TasksBoardDbContext context, 
        ILoggerFactory loggerFactory) : Repository<BoardNotice>(context, loggerFactory), IBoardNoticeRepository
    {
        public async Task<IEnumerable<BoardNotice>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.BoardId == boardId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.AuthorId == userId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAndBoardIdAsync(Guid userId, Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.AuthorId == userId && notice.BoardId == boardId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
