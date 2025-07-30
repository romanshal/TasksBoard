using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Constants;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Infrastructure.Consumers
{
    public class NewBoardInviteRequestEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<NewBoardInviteRequestEventConsumer> logger) : IConsumer<NewBoardInviteRequestEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<NewBoardInviteRequestEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardInviteRequestEvent> context)
        {
            if (context.Message.AccountId == Guid.Empty)
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardInviteRequestEvent));
                return;
            }

            var applicationEvent = new ApplicationEvent
            {
                EventId = (Guid)context.MessageId!,
                AccountId = context.Message.AccountId,
                EventType = nameof(NewBoardInviteRequestEvent),
                Payload = JsonSerializer.Serialize(context.Message)
            };

            _unitOfWork.GetRepository<ApplicationEvent>().Add(applicationEvent);

            var affectedRows = await _unitOfWork.SaveChangesAsync();
            if (affectedRows == 0 || applicationEvent.Id == Guid.Empty)
            {
                _logger.LogError(NotificationEventLogMessages.Error, context.MessageId);
            }

            _logger.LogInformation(NotificationEventLogMessages.Created, context.MessageId);
        }
    }
}
