using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardNoticeRepository : IRepository<BoardNotice>
    {
        Task<IEnumerable<BoardNotice>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAndBoardIdAsync(Guid userId, Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    }
}
