using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedPublicBoards
{
    public class GetPaginatedPublicBoardsQueryHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetPaginatedPublicBoardsQuery, PaginatedList<BoardForViewDto>>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardForViewDto>> Handle(GetPaginatedPublicBoardsQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetBoardRepository().CountPublicAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No boards entities in database.");
                return new PaginatedList<BoardForViewDto>([], request.PageIndex, request.PageSize);
            }

            var boards = await _unitOfWork.GetBoardRepository().GetPaginatedPublicAsync(request.PageIndex, request.PageSize, cancellationToken);

            var boardsDto = _mapper.Map<IEnumerable<Board>, IEnumerable<BoardForViewDto>>(boards,
                opt => opt.AfterMap((src, dest) =>
                {
                    foreach (var item in dest)
                    {
                        item.IsMember = src.First(board => board.Id == item.Id).BoardMembers.Any(member => member.AccountId == request.AccountId);
                    }
                }));

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
