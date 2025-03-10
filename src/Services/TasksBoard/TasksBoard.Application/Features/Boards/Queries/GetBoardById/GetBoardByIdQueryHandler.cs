using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public class GetBoardByIdQueryHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetBoardByIdQuery, BoardDto>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<BoardDto> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.Id, cancellationToken);

            var boardDto = _mapper.Map<BoardDto>(board);

            return boardDto;
        }
    }
}
