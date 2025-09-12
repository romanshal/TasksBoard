using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardMemberRepository : IRepository<BoardMember, BoardMemberId>
    {
        Task<IEnumerable<BoardMember>> GetByBoardIdAsync(BoardId boardId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BoardMember>> GetPaginatedByBoardIdAsync(BoardId boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<BoardMember?> GetByBoardIdAndAccountIdAsync(BoardId boardId, Guid accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BoardMember>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<int> CountByBoardIdAsync(BoardId boardId, CancellationToken cancellationToken);
    }
}
