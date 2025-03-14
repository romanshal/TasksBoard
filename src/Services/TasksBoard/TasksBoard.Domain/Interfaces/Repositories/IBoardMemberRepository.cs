using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardMemberRepository : IRepository<BoardMember>
    {
        Task<IEnumerable<BoardMember>> GetPaginatedByBoardIdAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<BoardMember?> GetByBoardIdAndUserIdAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default);
        Task<int> CountByBoardIdAsync(Guid boardId, CancellationToken cancellationToken);
    }
}
