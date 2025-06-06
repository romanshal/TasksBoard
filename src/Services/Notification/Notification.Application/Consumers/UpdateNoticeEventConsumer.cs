using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Application.Consumers
{
    public class UpdateNoticeEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<UpdateNoticeEventConsumer> logger) : IConsumer<UpdateNoticeEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateNoticeEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<UpdateNoticeEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning($"No members ids for save in '{nameof(UpdateNoticeEvent)}' event.");
                return;
            }

            foreach (var memberId in context.Message.BoardMembersIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = (Guid)context.MessageId!,
                    AccountId = memberId,
                    EventType = nameof(UpdateNoticeEvent),
                    Payload = JsonSerializer.Serialize(context.Message)
                };

                _unitOfWork.GetRepository<ApplicationEvent>().Add(applicationEvent, false);
            }

            var result = await _unitOfWork.SaveChangesAsync();

            if (result == 0)
            {
                _logger.LogError($"Error when create new event with id '{context.MessageId}'.");
            }

            _logger.LogInformation($"Event with id '{context.MessageId!}' created.");
        }
    }
}
