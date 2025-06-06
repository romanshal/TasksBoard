using EventBus.Messages.Events;

namespace Common.Blocks.Interfaces.Services
{
    public interface IOutboxService
    {
        Task<Guid> CreateNewOutboxEvent<T>(T newEvent, CancellationToken cancellationToken = default) where T : BaseEvent;
    }
}
