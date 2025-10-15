using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class UpdateNoticeEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<UpdateNoticeEvent> notificationBuilder,
        ILogger<UpdateNoticeEventConsumer> logger) : IConsumer<UpdateNoticeEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<UpdateNoticeEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<UpdateNoticeEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<UpdateNoticeEvent> context)
        {
            if (!context.Message.UsersInterested.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(UpdateNoticeEvent));
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
