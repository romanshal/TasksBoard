using Common.Blocks.Interfaces.Repositories;
using Common.Outbox.Entities;
using Common.Outbox.ValueObjects;

namespace Common.Outbox.Interfaces.Repositories
{
    public interface IOutboxEventRepository : IRepository<OutboxEvent, OutboxId>
    {
        Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default);
    }
}
