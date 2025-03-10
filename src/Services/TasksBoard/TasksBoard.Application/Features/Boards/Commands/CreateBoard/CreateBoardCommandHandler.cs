using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Commands.CreateBoard
{
    public class CreateBoardCommandHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateBoardCommand, Guid>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = _mapper.Map<Board>(request);

            await _unitOfWork.GetRepository<Board>().Add(board, true, cancellationToken);

            if (board.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board.");
                throw new ArgumentException(nameof(board));
            }

            return board.Id;
        }
    }
}
