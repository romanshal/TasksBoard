using Common.Blocks.Interfaces.Repositories;
using Notification.Domain.Entities;

namespace Notification.Domain.Interfaces.Repositories
{
    public interface IApplicationEventRepository : IRepository<ApplicationEvent>
    {
        Task<IEnumerable<ApplicationEvent>> GetPaginatedByAccountIdAsync(Guid accountId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationEvent>> GetNewPaginatedByAccountIdAsync(Guid accountId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationEvent>> GetNewByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationEvent>> GetByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationEvent>> GetNewByCreatedDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> CountByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<int> CountNewByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    }
}
