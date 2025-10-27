using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardAccessRequestRepository : IRepository<BoardAccessRequest, BoardAccessId>
    {
        Task<IEnumerable<BoardAccessRequest>> GetByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken);

        Task<BoardAccessRequest?> GetByBoardIdAndAccountId(
            BoardId boardId,
            AccountId accountId,
            CancellationToken cancellationToken);

        Task<IEnumerable<BoardAccessRequest>> GetByAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default);
    }
}
