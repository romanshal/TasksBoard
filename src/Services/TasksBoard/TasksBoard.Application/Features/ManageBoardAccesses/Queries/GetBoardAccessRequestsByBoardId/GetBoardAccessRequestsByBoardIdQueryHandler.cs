using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> logger,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardAccessRequestsByBoardIdQuery, Result<IEnumerable<BoardAccessRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> _logger = logger;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<IEnumerable<BoardAccessRequestDto>>> Handle(GetBoardAccessRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardId = BoardId.Of(request.BoardId);

            var boardExist = await _unitOfWork.GetBoardRepository().ExistAsync(boardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardAccessRequestDto>>(BoardErrors.NotFound);
            }

            var boardAccessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAsync(boardId, cancellationToken);

            var boardAccessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(boardAccessRequests);

            await _profileHandler.Handle(
                boardAccessRequestsDto,
                x => x.AccountId,
                (x, u, e) => { x.AccountName = u; x.AccountEmail = e!; },
                cancellationToken);

            return Result.Success(boardAccessRequestsDto);
        }
    }
}
