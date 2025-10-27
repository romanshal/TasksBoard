using Common.Blocks.Repositories;
using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.Repositories;
using Notification.Domain.ValueObjects;
using Notification.Infrastructure.Data.Contexts;

namespace Notification.Infrastructure.Repositories
{
    public class ApplicationEventRepository(
        NotificationDbContext context,
        ILoggerFactory loggerFactory) : Repository<ApplicationEvent, ApplicationEventId>(context, loggerFactory), IApplicationEventRepository
    {
        public async Task<IEnumerable<ApplicationEvent>> GetPaginatedByAccountIdAsync(
            AccountId accountId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(appEvent => appEvent.AccountId == accountId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationEvent>> GetNewByAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken)
        {
            return await DbSet
                .AsNoTracking()
                .Where(appEvent => appEvent.AccountId == accountId && !appEvent.Read)
                .OrderByDescending(appEvent => appEvent.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        //TODO: change this to VO
        public async Task<IEnumerable<ApplicationEvent>> GetByIdsAsync(
            IEnumerable<ApplicationEventId> ids,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(appEvent => ids.Contains(appEvent.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationEvent>> GetNewByCreatedDateAsync(
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.
                AsNoTracking()
                .Where(appEvent => appEvent.CreatedAt > date)
                .OrderBy(appEvent => appEvent.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationEvent>> GetNewPaginatedByAccountIdAsync(
            AccountId accountId,
            int pageIndex = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(appEvent => appEvent.AccountId == accountId && !appEvent.Read)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(appEvent => appEvent.AccountId == accountId, cancellationToken);
        }

        public async Task<int> CountNewByAccountIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(appEvent => !appEvent.Read)
                .CountAsync(appEvent => appEvent.AccountId == accountId, cancellationToken);
        }
    }
}
