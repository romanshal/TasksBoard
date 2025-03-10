using AutoMapper;
using Common.Blocks.Exceptons;
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
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.Id}' was not found.");
                throw new NotFoundException<Board>($"Board with id '{request.Id}' was not found.");
            }

            var boardDto = _mapper.Map<BoardDto>(board);

            return boardDto;
        }
    }
}
