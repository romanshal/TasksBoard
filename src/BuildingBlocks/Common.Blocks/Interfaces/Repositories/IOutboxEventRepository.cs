using Common.Blocks.Entities;

namespace Common.Blocks.Interfaces.Repositories
{
    public interface IOutboxEventRepository : IRepository<OutboxEvent>
    {
        Task<IEnumerable<OutboxEvent>> GetCreatedEventsAsync(CancellationToken cancellationToken = default);
    }
}
