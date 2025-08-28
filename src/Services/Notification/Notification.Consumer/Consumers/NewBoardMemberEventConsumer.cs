using EventBus.Messages.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class NewBoardMemberEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<NewBoardMemberEvent> notificationBuilder,
        ILogger<NewBoardMemberEventConsumer> logger) : IConsumer<NewBoardMemberEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<NewBoardMemberEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<NewBoardMemberEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardMemberEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardMemberEvent));
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
