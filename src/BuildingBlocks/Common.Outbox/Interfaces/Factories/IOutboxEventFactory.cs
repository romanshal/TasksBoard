using Common.Outbox.Entities;
using EventBus.Messages.Abstraction.Events;

namespace Common.Outbox.Interfaces.Factories
{
    public interface IOutboxEventFactory
    {
        OutboxEvent Create<T>(T @event) where T : BaseEvent;
    }
}
