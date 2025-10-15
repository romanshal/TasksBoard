using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class NewBoardMemberPermissionsEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<NewBoardMemberPermissionsEvent> notificationBuilder,
        ILogger<NewBoardMemberPermissionsEventConsumer> logger) : IConsumer<NewBoardMemberPermissionsEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<NewBoardMemberPermissionsEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<NewBoardMemberPermissionsEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardMemberPermissionsEvent> context)
        {
            if (!context.Message.UsersInterested.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardMemberPermissionsEvent));
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
