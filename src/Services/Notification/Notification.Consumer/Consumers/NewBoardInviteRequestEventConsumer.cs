using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class NewBoardInviteRequestEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<NewBoardInviteRequestEvent> notificationBuilder,
        ILogger<NewBoardInviteRequestEventConsumer> logger) : IConsumer<NewBoardInviteRequestEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<NewBoardInviteRequestEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<NewBoardInviteRequestEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardInviteRequestEvent> context)
        {
            if (context.Message.AccountId == Guid.Empty)
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardInviteRequestEvent));
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
