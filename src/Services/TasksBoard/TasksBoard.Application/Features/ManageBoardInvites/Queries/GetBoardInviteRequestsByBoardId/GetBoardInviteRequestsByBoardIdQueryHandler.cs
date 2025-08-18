using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Services;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> logger,
        IUserProfileService profileService) : IRequestHandler<GetBoardInviteRequestsByBoardIdQuery, Result<IEnumerable<BoardInviteRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> _logger = logger;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<IEnumerable<BoardInviteRequestDto>>> Handle(GetBoardInviteRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardInviteRequestDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardInviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAsync(request.BoardId, cancellationToken);

            var boardInviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(boardInviteRequests);

            var userIds = boardInviteRequestsDto
                .SelectMany(req => new[] { req.ToAccountId, req.FromAccountId })
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var req in boardInviteRequestsDto)
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

            return Result.Success(boardInviteRequestsDto);
        }
    }
}
