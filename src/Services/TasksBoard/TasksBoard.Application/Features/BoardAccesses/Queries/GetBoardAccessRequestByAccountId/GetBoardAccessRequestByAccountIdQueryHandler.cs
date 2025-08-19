using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId
{
    public class GetBoardAccessRequestByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardAccessRequestByAccountIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetBoardAccessRequestByAccountIdQuery, Result<IEnumerable<BoardAccessRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardAccessRequestByAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<IEnumerable<BoardAccessRequestDto>>> Handle(GetBoardAccessRequestByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var accessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByAccountIdAsync(request.AccountId, cancellationToken);

            var accessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(accessRequests);

            var userIds = accessRequestsDto
                .SelectMany(req => new[] { req.AccountId })
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var req in accessRequestsDto)
                {
                    var isExist = userProfiles.TryGetValue(req.AccountId, out var profile);

                    if (isExist && profile is not null)
                    {
                        req.AccountName = profile.Username;
                        req.AccountEmail = profile.Email;
                    }
                }
            }
            return Result.Success(accessRequestsDto);
        }
    }
}
