using EventBus.Messages.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class ResolveAccessRequestEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<ResolveAccessRequestEvent> notificationBuilder,
        ILogger<ResolveAccessRequestEventConsumer> logger) : IConsumer<ResolveAccessRequestEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<ResolveAccessRequestEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<ResolveAccessRequestEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<ResolveAccessRequestEvent> context)
        {
            if (context.Message.AccountId == Guid.Empty)
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(ResolveAccessRequestEvent));
                return;
            }


            var notificationRequest = _notificationBuilder
                .WithContext(context)
                .WithAccounts([context.Message.AccountId.ToString()])
                .Build();

            await _grpcService.Handle(notificationRequest);

            _logger.LogInformation(NotificationEventLogMessages.Created, context.MessageId);
        }
    }
}
