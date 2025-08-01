﻿using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> logger) : IRequestHandler<GetBoardAccessRequestsByBoardIdQuery, Result<IEnumerable<BoardAccessRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardAccessRequestsByBoardIdQueryHandler> _logger = logger;

        public async Task<Result<IEnumerable<BoardAccessRequestDto>>> Handle(GetBoardAccessRequestsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardAccessRequestDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardAccessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAsync(request.BoardId, cancellationToken);

            var boardAccessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(boardAccessRequests);

            return Result.Success(boardAccessRequestsDto);
        }
    }
}
