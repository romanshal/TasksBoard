using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Services;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public class GetBoardInviteRequestByToAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetBoardInviteRequestByToAccountIdQuery, Result<IEnumerable<BoardInviteRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<IEnumerable<BoardInviteRequestDto>>> Handle(GetBoardInviteRequestByToAccountIdQuery request, CancellationToken cancellationToken)
        {
            var inviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByToAccountIdAsync(request.AccountId, cancellationToken);

            var inviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(inviteRequests);

            var userIds = inviteRequestsDto
                .SelectMany(req => new[] { req.ToAccountId, req.FromAccountId })
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var req in inviteRequestsDto)
                {
                    if (userProfiles.TryGetValue(req.ToAccountId, out var profileTo) && profileTo is not null)
                    {
                        req.ToAccountName = profileTo.Username;
                        req.ToAccountEmail = profileTo.Email;
                    }

                    if (userProfiles.TryGetValue(req.FromAccountId, out var profileFrom) && profileFrom is not null)
                    {
                        req.FromAccountName = profileFrom.Username;
                    }
                }
            }

            return Result.Success(inviteRequestsDto);
        }
    }
}
