using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Interfaces.Repositories
{
    public interface IBoardAccessRequestRepository : IRepository<BoardAccessRequest>
    {
        Task<IEnumerable<BoardAccessRequest>> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken);
        Task<BoardAccessRequest?> GetByBoardIdAndAccountId(Guid boardId, Guid accountId, CancellationToken cancellationToken);
        Task<IEnumerable<BoardAccessRequest>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    }
}
