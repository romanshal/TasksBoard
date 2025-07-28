using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> logger) : IRequestHandler<GetBoardInviteRequestsByBoardIdQuery, Result<IEnumerable<BoardInviteRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> _logger = logger;

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

            return Result.Success(boardInviteRequestsDto);
        }
    }
}
