using Chat.Domain.Entities;
using Chat.Domain.Interfaces.Repositories;
using Chat.Infrastructure.Data.Contexts;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Infrastructure.Repositories
{
    public class BoardMessageRepository(
        ChatDbContext context,
        ILoggerFactory loggerFactory) : Repository<BoardMessage>(context, loggerFactory), IBoardMessageRepository
    {
        public async Task<IEnumerable<BoardMessage>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(message => message.BoardId == boardId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
