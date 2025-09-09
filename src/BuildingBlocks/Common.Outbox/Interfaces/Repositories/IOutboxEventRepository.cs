using Common.Blocks.Interfaces.Repositories;
using Common.Outbox.Entities;

namespace Common.Outbox.Interfaces.Repositories
{
    public interface IOutboxEventRepository : IRepository<OutboxEvent>
    {
        Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default);
    }
}
