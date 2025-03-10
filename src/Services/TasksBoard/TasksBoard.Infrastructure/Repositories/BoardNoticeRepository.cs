using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardNoticeRepository(DbContext context, ILoggerFactory loggerFactory) : Repository<BoardNotice>(context, loggerFactory), IBoardNoticeRepository
    {
        public async Task<IEnumerable<BoardNotice>> GetPaginatedAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(notice => notice.BoardId == boardId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
