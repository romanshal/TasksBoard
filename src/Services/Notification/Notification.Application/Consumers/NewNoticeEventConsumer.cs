using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Domain.Entities;
using System.Text.Json;

namespace Notification.Application.Consumers
{
    public class NewNoticeEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<NewNoticeEventConsumer> logger) : IConsumer<NewNoticeEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<NewNoticeEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<NewNoticeEvent> context)
        {
            if (!context.Message.BoardMembersIds.Any())
            {
                _logger.LogWarning($"No members ids for save in '{nameof(NewNoticeEvent)}' event.");
                return;
            }

            foreach (var memberId in context.Message.BoardMembersIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = (Guid)context.MessageId!,
                    AccountId = memberId,
                    EventType = nameof(NewNoticeEvent),
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
