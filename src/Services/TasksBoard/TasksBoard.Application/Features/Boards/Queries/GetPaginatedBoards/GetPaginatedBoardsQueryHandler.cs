using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards
{
    public class GetPaginatedBoardsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardsQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardsQuery, PaginatedList<BoardForViewDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardsQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardForViewDto>> Handle(GetPaginatedBoardsQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetRepository<Board>().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No boards entities in database.");
                return new PaginatedList<BoardForViewDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetRepository<Board>().GetPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardForViewDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
