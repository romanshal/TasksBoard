using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Domain.Constants;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Domain.ValueObjects;

namespace Notification.Application.Features.NotificationsGrpc.Commands.CreateNotifications
{
    public class CreateNotificationsCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateNotificationsCommandHandler> logger) : IRequestHandler<CreateNotificationsCommand, Result>
    {
        public async Task<Result> Handle(CreateNotificationsCommand request, CancellationToken cancellationToken)
        {
            foreach (var accontId in request.AccountIds)
            {
                var applicationEvent = new ApplicationEvent
                {
                    EventId = request.EventId,
                    EventType = request.EventType,
                    AccountId = accontId,
                    Payload = request.Payload
                };

                unitOfWork.GetRepository<ApplicationEvent, ApplicationEventId>().Add(applicationEvent);
            }

            var affectedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                logger.LogError(NotificationEventLogMessages.Error, request.EventId);
                return Result.Failure(NotificationErrors.CantCreate);
            }

            return Result.Success();
        }
    }
}
