using Chat.Domain.Entities;
using Common.Blocks.Interfaces.Repositories;

namespace Chat.Domain.Interfaces.Repositories
{
    public interface IBoardMessageRepository : IRepository<BoardMessage>
    {
        Task<IEnumerable<BoardMessage>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    }
}
