using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> logger) : IRequestHandler<GetBoardAccessRequestsByBoardIdQuery, IEnumerable<BoardAccessRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> _logger = logger;

        public async Task<IEnumerable<BoardAccessRequestDto>> Handle(GetBoardAccessRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardAccessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAsync(request.BoardId, cancellationToken);

            var boardAccessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(boardAccessRequests);

            return boardAccessRequestsDto;
        }
    }
}
