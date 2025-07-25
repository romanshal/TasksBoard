﻿using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetPaginatedNotificationsByAccountId
{
    public class GetAllPaginatedNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllPaginatedNotificationsByAccountIdQueryHandler> logger) : IRequestHandler<GetAllPaginatedNotificationsByAccountIdQuery, PaginatedList<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllPaginatedNotificationsByAccountIdQueryHandler> _logger = logger;

        public async Task<PaginatedList<NotificationDto>> Handle(GetAllPaginatedNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetApplicationEventRepository().CountByAccountIdAsync(request.AccountId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No notfication entities in database.");
                return new PaginatedList<NotificationDto>([], request.PageIndex, request.PageSize);
            }

            var notifications = await _unitOfWork.GetApplicationEventRepository().GetPaginatedByAccountIdAsync(request.AccountId, request.PageIndex, request.PageSize, cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return notificationsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
