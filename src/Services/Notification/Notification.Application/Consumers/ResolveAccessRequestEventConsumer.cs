using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using System.Text.Json;

namespace Notification.Application.Consumers
{
    public class ResolveAccessRequestEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<ResolveAccessRequestEventConsumer> logger) : IConsumer<ResolveAccessRequestEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ResolveAccessRequestEventConsumer> _logger = logger;
        public async Task Consume(ConsumeContext<ResolveAccessRequestEvent> context)
        {
            if (context.Message.AccountId == Guid.Empty)
            {
                _logger.LogWarning($"No members ids for save in '{nameof(ResolveAccessRequestEvent)}' event.");
                return;
            }

            var applicationEvent = new ApplicationEvent
            {
                EventId = (Guid)context.MessageId!,
                AccountId = context.Message.AccountId,
                EventType = nameof(ResolveAccessRequestEvent),
                Payload = JsonSerializer.Serialize(context.Message)
            };

            _unitOfWork.GetRepository<ApplicationEvent>().Add(applicationEvent, false);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result == 0)
            {
                _logger.LogError($"Error when create new event with id '{context.MessageId}'.");
            }

            _logger.LogInformation($"Event with id '{context.MessageId!}' created.");
        }
    }
}
