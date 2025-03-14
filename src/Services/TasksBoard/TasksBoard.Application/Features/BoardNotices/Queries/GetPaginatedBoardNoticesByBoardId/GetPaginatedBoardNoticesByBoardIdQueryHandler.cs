using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId
{
    public class GetPaginatedBoardNoticesByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardNoticesByBoardIdQuery, PaginatedList<BoardNoticeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardNoticeDto>> Handle(GetPaginatedBoardNoticesByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetBoardNoticeRepository().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
