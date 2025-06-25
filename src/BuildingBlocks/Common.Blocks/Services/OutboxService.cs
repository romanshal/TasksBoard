using Common.Blocks.Constants;
using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Interfaces.UnitOfWorks;
using EventBus.Messages.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Common.Blocks.Services
{
    public class OutboxService(
        IUnitOfWorkBase unitOfWork,
        ILogger<OutboxService> logger) : IOutboxService
    {
        private readonly IUnitOfWorkBase _unitOfWork = unitOfWork;
        private readonly ILogger<OutboxService> _logger = logger;

        public async Task<Guid> CreateNewOutboxEvent<T>(T newEvent, CancellationToken cancellationToken = default) where T : BaseEvent
        {
            var outboxEvent = new OutboxEvent
            {
                EventType = typeof(T).Name!,
                Payload = JsonSerializer.Serialize(newEvent),
                Status = OutboxEventStatuses.Created
            };

            await _unitOfWork.GetRepository<OutboxEvent>().Add(outboxEvent, true, cancellationToken);

            if (outboxEvent.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new outbox event.");
                throw new ArgumentException(nameof(outboxEvent));
            }

            return outboxEvent.Id;
        }
    }
}
