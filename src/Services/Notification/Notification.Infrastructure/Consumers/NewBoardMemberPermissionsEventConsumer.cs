using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Constants;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Infrastructure.Consumers
{
    public class NewBoardMemberPermissionsEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<NewBoardMemberPermissionsEventConsumer> logger) : IConsumer<NewBoardMemberPermissionsEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<NewBoardMemberPermissionsEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardMemberPermissionsEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardMemberPermissionsEvent));
                return;
            }

            foreach (var memberId in context.Message.BoardMembersIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = (Guid)context.MessageId!,
                    AccountId = memberId,
                    EventType = nameof(NewBoardMemberPermissionsEvent),
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
