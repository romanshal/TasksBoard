﻿using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetPaginatedBoardMembersByBoardId
{
    public class GetBoardMembersByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMembersByBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardMembersByBoardIdQuery, PaginatedList<BoardMemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMembersByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        public async Task<PaginatedList<BoardMemberDto>> Handle(GetPaginatedBoardMembersByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var count = await _unitOfWork.GetBoardMemberRepository().CountByBoardIdAsync(request.BoardId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board membres entities in database.");
                return new PaginatedList<BoardMemberDto>([], request.PageIndex, request.PageSize);
            }

            var boardMembers = await _unitOfWork.GetBoardMemberRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardMembersDto = _mapper.Map<IEnumerable<BoardMemberDto>>(boardMembers);

            return boardMembersDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
