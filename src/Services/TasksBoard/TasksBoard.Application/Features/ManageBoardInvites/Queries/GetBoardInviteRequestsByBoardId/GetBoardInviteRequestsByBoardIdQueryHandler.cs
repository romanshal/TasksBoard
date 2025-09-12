using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Factories;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> logger,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardInviteRequestsByBoardIdQuery, Result<IEnumerable<BoardInviteRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> _logger = logger;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<IEnumerable<BoardInviteRequestDto>>> Handle(GetBoardInviteRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardId = BoardId.Of(request.BoardId);

            var boardExist = await _unitOfWork.GetBoardRepository().ExistAsync(boardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardInviteRequestDto>>(BoardErrors.NotFound);
            }

            var boardInviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAsync(boardId, cancellationToken);

            var boardInviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(boardInviteRequests);

            await _profileHandler.HandleMany(
                [
                    UserProfileMappingFactory.Create(
                    boardInviteRequestsDto,
                    x => x.ToAccountId,
                    (x, u, e) => { x.ToAccountName = u; x.ToAccountEmail = e!; }),

                    UserProfileMappingFactory.Create(
                        boardInviteRequestsDto,
                        x => x.FromAccountId,
                        (x, u, _) => { x.FromAccountName = u; }),
                ], cancellationToken);

            return Result.Success(boardInviteRequestsDto);
        }
    }
}
