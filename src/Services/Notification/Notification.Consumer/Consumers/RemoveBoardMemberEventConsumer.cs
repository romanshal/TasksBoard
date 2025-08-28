using EventBus.Messages.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class RemoveBoardMemberEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<RemoveBoardMemberEvent> notificationBuilder,
        ILogger<RemoveBoardMemberEventConsumer> logger) : IConsumer<RemoveBoardMemberEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<RemoveBoardMemberEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<RemoveBoardMemberEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<RemoveBoardMemberEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(RemoveBoardMemberEvent));
                return;
            }

            var notificationRequest = _notificationBuilder
                .WithContext(context)
                .WithAccounts(context.Message.BoardMembersIds.Select(id => id.ToString()))
                .Build();

            await _grpcService.Handle(notificationRequest);

            _logger.LogInformation(NotificationEventLogMessages.Created, context.MessageId);
        }
    }
}
