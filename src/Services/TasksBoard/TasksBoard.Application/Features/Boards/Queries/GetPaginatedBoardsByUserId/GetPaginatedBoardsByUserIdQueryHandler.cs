using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId
{
    public class GetPaginatedBoardsByUserIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardsByUserIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardsByUserIdQuery, PaginatedList<BoardDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardsByUserIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardDto>> Handle(GetPaginatedBoardsByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Board> boards;
            int count;

            if (string.IsNullOrEmpty(request.Query))
            {
                count = await _unitOfWork.GetBoardRepository().CountByUserIdAsync(request.UserId, cancellationToken);
                if (count == 0)
                {
                    _logger.LogInformation("No boards entities in database.");
                    return new PaginatedList<BoardDto>([], request.PageIndex, request.PageSize);
                }

                boards = await _unitOfWork.GetBoardRepository().GetPaginatedByUserIdAsync(request.UserId, request.PageIndex, request.PageSize, cancellationToken);
            }
            else
            {
                count = await _unitOfWork.GetBoardRepository().CountByUserIdAndQueryAsync(request.UserId, request.Query, cancellationToken);
                if (count == 0)
                {
                    _logger.LogInformation("No boards entities in database.");
                    return new PaginatedList<BoardDto>([], request.PageIndex, request.PageSize);
                }

                boards = await _unitOfWork.GetBoardRepository().GetPaginatedByUserIdAndQueryAsync(request.UserId, request.Query, request.PageIndex, request.PageSize, cancellationToken);
            }

            var boardsDto = _mapper.Map<IEnumerable<BoardDto>>(boards);

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
