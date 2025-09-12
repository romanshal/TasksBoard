using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardInviteRequestRepository : IRepository<BoardInviteRequest, BoardInviteId>
    {
        Task<BoardInviteRequest?> GetByBoardIdAndToAccountIdAsync(
            BoardId boardId,
            Guid toAccountId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByBoardIdAsync(
            BoardId boardId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByToAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BoardInviteRequest>> GetByFromAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);
    }
}
