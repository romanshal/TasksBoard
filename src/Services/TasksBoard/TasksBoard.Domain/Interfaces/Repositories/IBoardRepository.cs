using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardRepository : IRepository<Board, BoardId>
    {
        Task<Board?> GetAsync(
            BoardId id,
            bool noTracking = true,
            bool include = true,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(
            AccountId userId, 
            int pageIndex = 1, 
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(
            AccountId userId, 
            string query, 
            int pageIndex = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Board>> GetPaginatedPublicAsync(
            int pageIndex = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default);

        Task<bool> HasAccessAsync(
            BoardId boardId,
            AccountId userId, 
            CancellationToken cancellationToken = default);

        Task<int> CountByUserIdAsync(
            AccountId userId, 
            CancellationToken cancellationToken = default);

        Task<int> CountByUserIdAndQueryAsync(
            AccountId userId, 
            string query, 
            CancellationToken cancellationToken = default);

        Task<int> CountPublicAsync(CancellationToken cancellationToken = default);
    }
}
