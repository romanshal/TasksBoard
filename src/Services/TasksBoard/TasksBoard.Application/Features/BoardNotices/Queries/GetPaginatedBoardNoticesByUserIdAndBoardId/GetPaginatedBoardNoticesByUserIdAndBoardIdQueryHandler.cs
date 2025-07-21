﻿using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId
{
    public class GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardNoticesByUserIdAndBoardIdQuery, PaginatedList<BoardNoticeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardNoticeDto>> Handle(GetPaginatedBoardNoticesByUserIdAndBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var count = await _unitOfWork.GetBoardNoticeRepository().CountByBoardIdAndUserIdAsync(request.BoardId, request.UserId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize);
            }

            var boardNotices = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAndBoardIdAsync(request.UserId, request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotices);

            return boardNoticesDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
