﻿using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetNewNotificationsByAccountIdQueryHandler> logger) : IRequestHandler<GetNewNotificationsByAccountIdQuery, Result<IEnumerable<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetNewNotificationsByAccountIdQueryHandler> _logger = logger;

        public async Task<Result<IEnumerable<NotificationDto>>> Handle(GetNewNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.GetApplicationEventRepository().GetNewByAccountIdAsync(request.AccountId, cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return Result.Success(notificationsDto);
        }
    }
}
