using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

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
            var count = await _unitOfWork.GetBoardNoticeRepository().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAndBoardIdAsync(request.UserId, request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
