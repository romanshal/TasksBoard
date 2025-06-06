using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardInviteRequestRepository : IRepository<BoardInviteRequest>
    {
        Task<BoardInviteRequest?> GetByBoardIdAndToAccountIdAsync(Guid boardId, Guid toAccountId, CancellationToken cancellationToken);
        Task<IEnumerable<BoardInviteRequest>> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken);
        Task<IEnumerable<BoardInviteRequest>> GetByToAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BoardInviteRequest>> GetByFromAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    }
}
