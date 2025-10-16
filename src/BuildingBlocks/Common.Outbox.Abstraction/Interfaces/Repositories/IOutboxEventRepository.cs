using Common.Blocks.Interfaces.Repositories;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.ValueObjects;

namespace Common.Outbox.Abstraction.Interfaces.Repositories
{
    public interface IOutboxEventRepository : IRepository<OutboxEvent, OutboxId>
    {
        Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default);
    }
}
