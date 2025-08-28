using Common.gRPC.Protos;
using EventBus.Messages.Events;
using MassTransit;

namespace Notification.Consumer.Builders
{
    public interface IGrpcRequestBuilder<TEvent> where TEvent: BaseEvent
    {
        GrpcRequestBuilder<TEvent> WithContext<TContext>(TContext context) where TContext : ConsumeContext<TEvent>;
        GrpcRequestBuilder<TEvent> WithAccounts(IEnumerable<string> accountIds);
        CreateNotificationRequest Build();
    }
}
