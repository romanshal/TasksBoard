using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Application.Models;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices
{
    public class GetPaginatedBoardNoticesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedListQuery<BoardNoticeDto>, PaginatedList<BoardNoticeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardNoticeDto>> Handle(GetPaginatedListQuery<BoardNoticeDto> request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetRepository<BoardNotice>().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize);
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticeDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotice);

            return boardNoticeDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
