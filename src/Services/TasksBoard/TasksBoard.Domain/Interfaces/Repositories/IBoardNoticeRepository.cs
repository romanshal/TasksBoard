using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardNoticeRepository : IRepository<BoardNotice, BoardNoticeId>
    {
        Task<IEnumerable<BoardNotice>> GetPaginatedByBoardIdAsync(
            BoardId boardId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAsync(
            Guid userId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardNotice>> GetPaginatedByUserIdAndBoardIdAsync(
            Guid userId,
            BoardId boardId,
            int pageIndex = 1,
            int pageSize = 10, CancellationToken cancellationToken = default);

        Task<int> CountByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken = default);

        Task<int> CountByUserIdAsync
            (Guid userId,
            CancellationToken cancellationToken = default);

        Task<int> CountByBoardIdAndUserIdAsync(
            BoardId boardId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
