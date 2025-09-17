using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Application.Handlers;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetNewNotificationsByAccountIdQueryHandler> logger,
        UserProfileHandler profileHandler) : IRequestHandler<GetNewNotificationsByAccountIdQuery, Result<IEnumerable<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetNewNotificationsByAccountIdQueryHandler> _logger = logger;
        private readonly UserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<IEnumerable<NotificationDto>>> Handle(GetNewNotificationsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.GetApplicationEventRepository().GetNewByAccountIdAsync(AccountId.Of(request.AccountId), cancellationToken);

            var notificationsDto = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            await _profileHandler.Handle(notificationsDto, cancellationToken);

            return Result.Success(notificationsDto);
        }
    }
}
