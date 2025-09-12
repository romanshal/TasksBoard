using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> logger,
        IUserProfileService profileService) : IRequestHandler<GetBoardAccessRequestsByBoardIdQuery, Result<IEnumerable<BoardAccessRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> _logger = logger;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<IEnumerable<BoardAccessRequestDto>>> Handle(GetBoardAccessRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board, BoardId>().ExistAsync(BoardId.Of(request.BoardId), cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardAccessRequestDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardAccessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAsync(BoardId.Of(request.BoardId), cancellationToken);

            var boardAccessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(boardAccessRequests);

            var userIds = boardAccessRequestsDto
                .SelectMany(req => new[] { req.AccountId })
                .Where(id => id != Guid.Empty)
                .ToHashSet();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var req in boardAccessRequestsDto)
                {
                    var isExist = userProfiles.TryGetValue(req.AccountId, out var profile);

                    if (isExist && profile is not null)
                    {
                        req.AccountName = profile.Username;
                        req.AccountEmail = profile.Email;
                    }
                }
            }

            return Result.Success(boardAccessRequestsDto);
        }
    }
}
