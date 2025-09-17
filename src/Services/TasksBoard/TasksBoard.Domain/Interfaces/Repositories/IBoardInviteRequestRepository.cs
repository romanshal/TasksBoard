using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardInviteRequestRepository : IRepository<BoardInviteRequest, BoardInviteId>
    {
        Task<BoardInviteRequest?> GetByBoardIdAndToAccountIdAsync(
            BoardId boardId,
            AccountId toAccountId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByToAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByFromAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default);
    }
}
