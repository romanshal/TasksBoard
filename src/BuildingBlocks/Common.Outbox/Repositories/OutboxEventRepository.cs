using Common.Blocks.Constants;
using Common.Blocks.Repositories;
using Common.Outbox.Entities;
using Common.Outbox.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Outbox.Repositories
{
    public class OutboxEventRepository(
        DbContext context,
        ILoggerFactory loggerFactory) : Repository<OutboxEvent>(context, loggerFactory), IOutboxEventRepository
    {
        public async Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(outbox => outbox.Status == OutboxEventStatuses.Created)
                .ToListAsync(cancellationToken);
        }
    }
}
