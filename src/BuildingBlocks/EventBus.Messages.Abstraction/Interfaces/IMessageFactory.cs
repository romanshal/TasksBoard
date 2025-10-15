using EventBus.Messages.Abstraction.Events;

namespace EventBus.Messages.Abstraction.Interfaces
{
    public interface IMessageFactory
    {
        IAsyncEnumerable<TEvent> Generate<TEvent>(Guid entityId) where TEvent : BaseEvent, new();
    }
}
