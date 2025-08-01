﻿using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Constants;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Infrastructure.Consumers
{
    public class NewBoardMemberEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<NewBoardMemberEventConsumer> logger) : IConsumer<NewBoardMemberEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<NewBoardMemberEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewBoardMemberEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning(NotificationEventLogMessages.NoMemberIds, nameof(NewBoardMemberEvent));
                return;
            }

            foreach (var memberId in context.Message.BoardMembersIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = (Guid)context.MessageId!,
                    AccountId = memberId,
                    EventType = nameof(NewBoardMemberEvent),
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
