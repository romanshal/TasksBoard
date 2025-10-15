using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using Notification.Domain.Constants;

namespace Notification.Consumer.Consumers
{
    public class NewBoardAccessRequestEventConsumer(
        IGrpcService grpcService,
        IGrpcRequestBuilder<NewBoardAccessRequestEvent> notificationBuilder,
        ILogger<NewBoardAccessRequestEventConsumer> logger) : IConsumer<NewBoardAccessRequestEvent>
    {
        private readonly IGrpcService _grpcService = grpcService;
        private readonly IGrpcRequestBuilder<NewBoardAccessRequestEvent> _notificationBuilder = notificationBuilder;
        private readonly ILogger<NewBoardAccessRequestEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardAccessRequestEvent> context)
        {
            if (!context.Message.UsersInterested.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardAccessRequestEvent));
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
