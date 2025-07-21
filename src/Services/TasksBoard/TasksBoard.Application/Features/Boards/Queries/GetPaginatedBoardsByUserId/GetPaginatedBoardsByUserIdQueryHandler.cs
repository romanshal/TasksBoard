using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId
{
    public class GetPaginatedBoardsByUserIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardsByUserIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardsByUserIdQuery, PaginatedList<BoardForViewDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardsByUserIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedList<BoardForViewDto>> Handle(GetPaginatedBoardsByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Board> boards;
            int count;

            if (string.IsNullOrEmpty(request.Query))
            {
                count = await _unitOfWork.GetBoardRepository().CountByUserIdAsync(request.UserId, cancellationToken);
                if (count == 0)
                {
                    _logger.LogInformation("No boards entities in database.");
                    return new PaginatedList<BoardForViewDto>([], request.PageIndex, request.PageSize);
                }

                boards = await _unitOfWork.GetBoardRepository().GetPaginatedByUserIdAsync(request.UserId, request.PageIndex, request.PageSize, cancellationToken);
            }
            else
            {
                count = await _unitOfWork.GetBoardRepository().CountByUserIdAndQueryAsync(request.UserId, request.Query, cancellationToken);
                if (count == 0)
                {
                    _logger.LogInformation("No boards entities in database.");
                    return new PaginatedList<BoardForViewDto>([], request.PageIndex, request.PageSize);
                }

                boards = await _unitOfWork.GetBoardRepository().GetPaginatedByUserIdAndQueryAsync(request.UserId, request.Query, request.PageIndex, request.PageSize, cancellationToken);
            }

            var boardsDto = _mapper.Map<IEnumerable<Board>, IEnumerable<BoardForViewDto>>(boards,
                opt => opt.AfterMap((src, dest) =>
                {
                    foreach (var item in dest)
                    {
                        item.IsMember = src.First(board => board.Id == item.Id).BoardMembers.Any(member => member.AccountId == request.UserId);
                    }
                }));

            return boardsDto.ToPaginatedList(request.PageIndex, request.PageSize, count);
        }
    }
}
