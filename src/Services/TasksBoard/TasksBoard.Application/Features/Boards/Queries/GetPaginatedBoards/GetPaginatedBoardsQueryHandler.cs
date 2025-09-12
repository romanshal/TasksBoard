using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards
{
    public class GetPaginatedBoardsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardsQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardsQuery, Result<PaginatedList<BoardForViewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardsQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<PaginatedList<BoardForViewDto>>> Handle(GetPaginatedBoardsQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetBoardRepository().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No boards entities in database.");
                return Result.Success(PaginatedList<BoardForViewDto>.Empty(request.PageIndex, request.PageSize));
            }

            var boards = await _unitOfWork.GetBoardRepository().GetPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<BoardForViewDto>>(boards);

            return Result.Success(boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
