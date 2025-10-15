using EventBus.Messages.Abstraction.Events;
using EventBus.Messages.Abstraction.Interfaces;

namespace EventBus.Messages.Factories
{
    internal class MessageFactory : IMessageFactory
    {
        public async IAsyncEnumerable<TEvent> Generate<TEvent>(Guid entityId) where TEvent : BaseEvent, new()
        {
            yield return new TEvent();
        }
    }
}
