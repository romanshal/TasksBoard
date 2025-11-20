using Common.Blocks.Contexts;
using Common.Blocks.Repositories;
using Common.Outbox.Abstraction.Constants;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Abstraction.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Outbox.Repositories
{
    public class OutboxEventRepository<TContext>(
        TContext context,
        ILoggerFactory loggerFactory) : Repository<OutboxEvent, OutboxId>(context, loggerFactory), IOutboxEventRepository where TContext : CommonDbContext<TContext>
    {
        public async Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(outbox => outbox.Status == OutboxEventStatuses.Created)
                .ToListAsync(cancellationToken);
        }
    }
}
