using EventBus.Messages.Events;

namespace Common.Outbox.Interfaces.Services
{
    public interface IOutboxService
    {
        Task<Guid> CreateNewOutboxEvent<T>(T newEvent, CancellationToken cancellationToken = default) where T : BaseEvent;
    }
}
