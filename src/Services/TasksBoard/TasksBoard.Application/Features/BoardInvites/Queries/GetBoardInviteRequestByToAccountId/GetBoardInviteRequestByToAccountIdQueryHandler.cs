using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Factories;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public class GetBoardInviteRequestByToAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardInviteRequestByToAccountIdQuery, Result<IEnumerable<BoardInviteRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<IEnumerable<BoardInviteRequestDto>>> Handle(GetBoardInviteRequestByToAccountIdQuery request, CancellationToken cancellationToken)
        {
            var inviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByToAccountIdAsync(request.AccountId, cancellationToken);

            var inviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(inviteRequests);

            await _profileHandler.HandleMany(
            [
                UserProfileMappingFactory.Create(inviteRequestsDto, x => x.ToAccountId, (x, u, e) => { x.ToAccountName = u; x.ToAccountEmail = e!; }),
                UserProfileMappingFactory.Create(inviteRequestsDto, x => x.FromAccountId, (x, u, _) => { x.FromAccountName = u; }),
            ], cancellationToken);

            return Result.Success(inviteRequestsDto);
        }
    }
}
