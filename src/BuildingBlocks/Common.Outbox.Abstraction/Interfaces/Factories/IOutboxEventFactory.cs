using Common.Outbox.Abstraction.Entities;
using EventBus.Messages.Abstraction.Events;

namespace Common.Outbox.Abstraction.Interfaces.Factories
{
    public interface IOutboxEventFactory
    {
        OutboxEvent Create<T>(T @event) where T : BaseEvent;
    }
}
