using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetNewNotificationsByAccountIdQueryHandler> logger) : IRequestHandler<GetNewNotificationsByAccountIdQuery, IEnumerable<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetNewNotificationsByAccountIdQueryHandler> _logger = logger;

        public async Task<IEnumerable<NotificationDto>> Handle(GetNewNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.GetApplicationEventRepository().GetNewByAccountIdAsync(request.AccountId, cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return notificationsDto;
        }
    }
}
