using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Extensions;
using Notification.Domain.Constants;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Commands.SetNotificationsRead
{
    public class SetNotificationsReadCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<SetNotificationsReadCommandHandler> logger) : IRequestHandler<SetNotificationsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SetNotificationsReadCommandHandler> _logger = logger;

        public async Task<Result> Handle(SetNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.GetApplicationEventRepository().GetByIdsAsync(request.NotificationIds.ToValueObjectList(), cancellationToken);
            if (!notifications.Any())
            {
                return Result.Success();
            }

            foreach (var notification in notifications)
            {
                notification.Read = true;

                _unitOfWork.GetApplicationEventRepository().Update(notification);
            }

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                _logger.LogError("Can't update notifications.");
                return Result.Failure(NotificationErrors.CantUpdate);

                //throw new ArgumentException("Can't save notifications.");
            }

            return Result.Success();
        }
    }
}
