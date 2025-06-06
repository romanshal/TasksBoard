using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> logger) : IRequestHandler<GetBoardInviteRequestsByBoardIdQuery, IEnumerable<BoardInviteRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardInviteRequestsByBoardIdQueryHandler> _logger = logger;

        public async Task<IEnumerable<BoardInviteRequestDto>> Handle(GetBoardInviteRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardInviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAsync(request.BoardId, cancellationToken);

            var boardInviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(boardInviteRequests);

            return boardInviteRequestsDto;
        }
    }
}
