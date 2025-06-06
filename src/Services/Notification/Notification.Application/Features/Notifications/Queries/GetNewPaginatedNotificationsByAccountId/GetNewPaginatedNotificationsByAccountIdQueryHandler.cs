using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetNewPaginatedNotificationsByAccountId
{
    public class GetNewPaginatedNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetNewPaginatedNotificationsByAccountIdQueryHandler> logger) : IRequestHandler<GetNewPaginatedNotificationsByAccountIdQuery, PaginatedList<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetNewPaginatedNotificationsByAccountIdQueryHandler> _logger = logger;

        public async Task<PaginatedList<NotificationDto>> Handle(GetNewPaginatedNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetApplicationEventRepository().CountNewByAccountIdAsync(request.AccountId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation($"No new notfication entities in database for user '{request.AccountId}'.");
                return new PaginatedList<NotificationDto>([], request.PageIndex, request.PageSize);
            }

            var notifications = await _unitOfWork.GetApplicationEventRepository().GetNewPaginatedByAccountIdAsync(request.AccountId, request.PageIndex, request.PageSize, cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return notificationsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
