using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Outbox.Abstraction.Constants;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Extensions;
using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Common.Outbox.Services
{
    public class OutboxPublisherService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OutboxPublisherService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            using IServiceScope scope = serviceScopeFactory.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWorkBase>();

            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var outboxEvents = await unitOfWork.GetOutboxEventRepository().GetCreatedEventsAsync(stoppingToken);

                    foreach (var outboxEvent in outboxEvents)
                    {
                        var applicationEvent = GetEvent(outboxEvent);

                        await PublishEventAsync(applicationEvent, publishEndpoint, stoppingToken);

                        outboxEvent.Status = OutboxEventStatuses.Sent;

                        unitOfWork.GetOutboxEventRepository().Update(outboxEvent);

                        var affectedRows = await unitOfWork.SaveChangesAsync(stoppingToken);
                        if (affectedRows == 0)
                        {
                            throw new Exception("Can't update outbox entity.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Error in publishing outboxevent: {message}", ex.Message);
                }
                finally
                {
                    await Task.Delay(2500, stoppingToken);
                }
            }
        }

        private BaseEvent GetEvent(OutboxEvent outboxEvent)
        {
            return outboxEvent.EventType switch
            {
                nameof(NewNoticeEvent) => JsonSerializer.Deserialize<NewNoticeEvent>(outboxEvent.Payload)!,
                nameof(UpdateNoticeStatusEvent) => JsonSerializer.Deserialize<UpdateNoticeStatusEvent>(outboxEvent.Payload)!,
                nameof(NewBoardMemberEvent) => JsonSerializer.Deserialize<NewBoardMemberEvent>(outboxEvent.Payload)!,
                nameof(RemoveBoardMemberEvent) => JsonSerializer.Deserialize<RemoveBoardMemberEvent>(outboxEvent.Payload)!,
                nameof(ResolveAccessRequestEvent) => JsonSerializer.Deserialize<ResolveAccessRequestEvent>(outboxEvent.Payload)!,
                nameof(NewBoardInviteRequestEvent) => JsonSerializer.Deserialize<NewBoardInviteRequestEvent>(outboxEvent.Payload)!,
                nameof(NewBoardAccessRequestEvent) => JsonSerializer.Deserialize<NewBoardAccessRequestEvent>(outboxEvent.Payload)!,
                nameof(NewBoardMemberPermissionsEvent) => JsonSerializer.Deserialize<NewBoardMemberPermissionsEvent>(outboxEvent.Payload)!,
                nameof(UpdateNoticeEvent) => JsonSerializer.Deserialize<UpdateNoticeEvent>(outboxEvent.Payload)!,
                nameof(DeleteBoardEvent) => JsonSerializer.Deserialize<DeleteBoardEvent>(outboxEvent.Payload)!,
                _ => throw new Exception(),
            };
        }

        private async Task PublishEventAsync<T>(T applicationEvent, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken) where T : BaseEvent
        {
            var type = applicationEvent.GetType().Name;

            switch (type)
            {
                case nameof(NewNoticeEvent):
                    await publishEndpoint.Publish<NewNoticeEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(UpdateNoticeStatusEvent):
                    await publishEndpoint.Publish<UpdateNoticeStatusEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(NewBoardMemberEvent):
                    await publishEndpoint.Publish<NewBoardMemberEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(RemoveBoardMemberEvent):
                    await publishEndpoint.Publish<RemoveBoardMemberEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(ResolveAccessRequestEvent):
                    await publishEndpoint.Publish<ResolveAccessRequestEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(NewBoardInviteRequestEvent):
                    await publishEndpoint.Publish<NewBoardInviteRequestEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(NewBoardAccessRequestEvent):
                    await publishEndpoint.Publish<NewBoardAccessRequestEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(NewBoardMemberPermissionsEvent):
                    await publishEndpoint.Publish<NewBoardMemberPermissionsEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(UpdateNoticeEvent):
                    await publishEndpoint.Publish<UpdateNoticeEvent>(applicationEvent, cancellationToken);
                    break;
                case nameof(DeleteBoardEvent):
                    await publishEndpoint.Publish<DeleteBoardEvent>(applicationEvent, cancellationToken);
                    break;
            }
        }
    }
}
