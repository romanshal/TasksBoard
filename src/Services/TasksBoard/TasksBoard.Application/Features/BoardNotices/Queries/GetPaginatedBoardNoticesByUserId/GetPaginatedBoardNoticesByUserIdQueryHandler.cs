using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId
{
    public class GetPaginatedBoardNoticesByUserIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardNoticesByUserIdQuery, PaginatedList<BoardNoticeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardNoticeDto>> Handle(GetPaginatedBoardNoticesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetBoardNoticeRepository().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAsync(request.UserId, request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
