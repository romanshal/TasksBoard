using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoards
{
    public class GetBoardsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardsQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardsQuery, IEnumerable<BoardDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardsQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BoardDto>> Handle(GetBoardsQuery request, CancellationToken cancellationToken)
        {
            var boards = await _unitOfWork.GetRepository<Board>().GetAllAsync(cancellationToken);

            var boardDto = _mapper.Map<IEnumerable<BoardDto>>(boards);

            return boardDto;
        }
    }
}
