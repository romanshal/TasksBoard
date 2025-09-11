using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Application.Features.Notifications.Queries.GetPaginatedNotificationsByAccountId;
using Notification.Application.Handlers;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetAllPaginatedNotificationsByAccountId
{
    public class GetAllPaginatedNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllPaginatedNotificationsByAccountIdQueryHandler> logger,
        UserProfileHandler profileHandler) : IRequestHandler<GetAllPaginatedNotificationsByAccountIdQuery, Result<PaginatedList<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllPaginatedNotificationsByAccountIdQueryHandler> _logger = logger;
        private readonly UserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<PaginatedList<NotificationDto>>> Handle(GetAllPaginatedNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetApplicationEventRepository().CountByAccountIdAsync(request.AccountId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No notfication entities in database.");
                return Result.Success(PaginatedList<NotificationDto>.Empty(request.PageIndex, request.PageSize));
            }

            var notifications = await _unitOfWork.GetApplicationEventRepository().GetPaginatedByAccountIdAsync(request.AccountId, request.PageIndex, request.PageSize, cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            await _profileHandler.Handle(notificationsDto, cancellationToken);

            return Result.Success(notificationsDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
