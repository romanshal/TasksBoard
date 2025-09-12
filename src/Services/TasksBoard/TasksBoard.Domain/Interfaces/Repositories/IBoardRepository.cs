using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardRepository : IRepository<Board, BoardId>
    {
        Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(Guid userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<bool> HasAccessAsync(BoardId boardId, Guid userId, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAndQueryAsync(Guid userId, string query, CancellationToken cancellationToken = default);
        Task<int> CountPublicAsync(CancellationToken cancellationToken = default);
    }
}
