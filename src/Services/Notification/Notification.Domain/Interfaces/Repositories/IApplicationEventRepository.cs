using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using Notification.Domain.Entities;
using Notification.Domain.ValueObjects;

namespace Notification.Domain.Interfaces.Repositories
{
    public interface IApplicationEventRepository : IRepository<ApplicationEvent, ApplicationEventId>
    {
        Task<IEnumerable<ApplicationEvent>> GetPaginatedByAccountIdAsync(
            AccountId accountId, 
            int pageIndex = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ApplicationEvent>> GetNewPaginatedByAccountIdAsync(
            AccountId accountId, 
            int pageIndex = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ApplicationEvent>> GetNewByAccountIdAsync(
            AccountId accountId, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ApplicationEvent>> GetByIdsAsync(
            IEnumerable<ApplicationEventId> ids, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ApplicationEvent>> GetNewByCreatedDateAsync(
            DateTime date, 
            CancellationToken cancellationToken = default);

        Task<int> CountByAccountIdAsync(
            AccountId accountId, 
            CancellationToken cancellationToken = default);

        Task<int> CountNewByAccountIdAsync(
            AccountId accountId, 
            CancellationToken cancellationToken = default);
    }
}
