using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class NewNoticeEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<NewNoticeEvent> notificationBuilder,
        ILogger<NewNoticeEventConsumer> logger) : IConsumer<NewNoticeEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<NewNoticeEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<NewNoticeEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewNoticeEvent> context)
        {
            if (!context.Message.UsersInterested.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewNoticeEvent));
                return;
            }

            var notificationRequest = _notificationBuilder
                .WithContext(context)
                .WithAccounts(context.Message.UsersInterested.Select(id => id.ToString()))
                .Build();

            await _grpcService.Handle(notificationRequest);

            _logger.LogInformation(NotificationEventLogMessages.Created, context.MessageId);
        }
    }
}
