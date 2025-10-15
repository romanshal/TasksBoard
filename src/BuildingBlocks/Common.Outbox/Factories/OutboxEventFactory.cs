using Common.Outbox.Constants;
using Common.Outbox.Entities;
using Common.Outbox.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using Newtonsoft.Json;

namespace Common.Outbox.Factories
{
    internal class OutboxEventFactory : IOutboxEventFactory
    {
        public OutboxEvent Create<T>(T @event) where T : BaseEvent => new()
        {
            EventType = typeof(T).Name!,
            Payload = JsonConvert.SerializeObject(@event),
            Status = OutboxEventStatuses.Created
        };
    }
}
