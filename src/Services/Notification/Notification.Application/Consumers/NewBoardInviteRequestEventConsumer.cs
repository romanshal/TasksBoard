using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Application.Consumers
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
                _logger.LogWarning($"No members ids for save in '{nameof(NewBoardInviteRequestEvent)}' event.");
                return;
            }

            var applicationEvent = new ApplicationEvent
            {
                EventId = (Guid)context.MessageId!,
                AccountId = context.Message.AccountId,
                EventType = nameof(NewBoardInviteRequestEvent),
                Payload = JsonSerializer.Serialize(context.Message)
            };

            await _unitOfWork.GetRepository<ApplicationEvent>().Add(applicationEvent, true);

            if (applicationEvent.Id == Guid.Empty)
            {
                _logger.LogError($"Error when create new event with id '{context.MessageId}'.");
            }

            _logger.LogInformation($"Event with id '{context.MessageId!}' created.");
        }
    }
}
