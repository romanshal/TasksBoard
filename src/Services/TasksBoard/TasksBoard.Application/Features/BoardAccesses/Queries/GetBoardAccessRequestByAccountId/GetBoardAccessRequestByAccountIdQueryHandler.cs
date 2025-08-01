﻿using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId
{
    public class GetBoardAccessRequestByAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardAccessRequestByAccountIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardAccessRequestByAccountIdQuery, Result<IEnumerable<BoardAccessRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardAccessRequestByAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<IEnumerable<BoardAccessRequestDto>>> Handle(GetBoardAccessRequestByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var accessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByAccountIdAsync(request.AccountId, cancellationToken);

            var accessRequestsDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(accessRequests);

            return Result.Success(accessRequestsDto);
        }
    }
}
