﻿using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Commands.SetNotificationsRead
{
    public class SetNotificationsReadCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<SetNotificationsReadCommandHandler> logger) : IRequestHandler<SetNotificationsReadCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SetNotificationsReadCommandHandler> _logger = logger;

        public async Task<Unit> Handle(SetNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.GetApplicationEventRepository().GetByIdsAsync(request.NotificationIds, cancellationToken);
            if (!notifications.Any())
            {
                return Unit.Value;
            }

            foreach (var notification in notifications)
            {
                notification.Read = true;

                _unitOfWork.GetApplicationEventRepository().Update(notification);
            }

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if(affectedRows == 0)
            {
                _logger.LogError("Can't save notifications.");
                throw new ArgumentException("Can't save notifications.");
            }

            return Unit.Value;
        }
    }
}
