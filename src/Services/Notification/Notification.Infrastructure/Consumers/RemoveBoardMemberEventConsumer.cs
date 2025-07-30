using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Constants;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Infrastructure.Consumers
{
    public class RemoveBoardMemberEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<RemoveBoardMemberEventConsumer> logger) : IConsumer<RemoveBoardMemberEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<RemoveBoardMemberEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<RemoveBoardMemberEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(RemoveBoardMemberEvent));
                return;
            }

            foreach (var memberId in context.Message.BoardMembersIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = (Guid)context.MessageId!,
                    AccountId = memberId,
                    EventType = nameof(RemoveBoardMemberEvent),
                    Payload = JsonSerializer.Serialize(context.Message)
                };

                _unitOfWork.GetRepository<ApplicationEvent>().Add(applicationEvent);
            }

            var affectedRows = await _unitOfWork.SaveChangesAsync();
            if (affectedRows == 0)
            {
                _logger.LogError(NotificationEventLogMessages.Error, context.MessageId);
            }

            _logger.LogInformation(NotificationEventLogMessages.Created, context.MessageId);
        }
    }
}
