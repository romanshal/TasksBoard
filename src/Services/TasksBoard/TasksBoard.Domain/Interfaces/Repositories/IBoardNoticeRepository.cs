using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Domain.Interfaces.Repositories
{
    public interface IBoardNoticeRepository : IRepository<BoardNotice>
    {
        Task<IEnumerable<BoardNotice>> GetPaginatedAsync(Guid boardId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    }
}
