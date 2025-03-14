using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Queries.GetBoards;
using TasksBoard.Application.Models;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards
{
    public class GetPaginatedBoardsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardsQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedListQuery<BoardDto>, PaginatedList<BoardDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardsQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardDto>> Handle(GetPaginatedListQuery<BoardDto> request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetRepository<Board>().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No boards entities in database.");
                return new PaginatedList<BoardDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetRepository<Board>().GetPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
