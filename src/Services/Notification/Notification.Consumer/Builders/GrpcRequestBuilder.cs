using Common.gRPC.Protos;
using EventBus.Messages.Events;
using MassTransit;
using System.Text.Json;

namespace Notification.Consumer.Builders
{
    public class GrpcRequestBuilder<TEvent> : IGrpcRequestBuilder<TEvent> where TEvent: BaseEvent
    {
        private readonly CreateNotificationRequest _req = new();

        public GrpcRequestBuilder<TEvent> WithContext<TContext>(TContext context)
            where TContext : ConsumeContext<TEvent>
        {
            _req.EventId = context.MessageId.ToString();
            _req.EventType = context.Message.GetType().Name;
            _req.Payload = JsonSerializer.Serialize(context.Message);

            return this;
        }

        public GrpcRequestBuilder<TEvent> WithAccounts(IEnumerable<string> accountIds)
        {
            _req.AccountIds.AddRange(accountIds);
            return this;
        }

        public CreateNotificationRequest Build() => _req;
    }
}
