using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.Boards.Commands.CreateBoard
{
    public class CreateBoardCommandHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateBoardCommand, Guid>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await CreateBoardAsync(request, cancellationToken);

            var boardMember = await CreateBoardMemberAsync(board.OwnerId, board.Id, request.OwnerNickname, cancellationToken);

            await AddPermissionsAsync(boardMember.Id, cancellationToken);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows == 0)
            {
                _logger.LogError("Can't create new board. No rows were affected.");
                throw new InvalidOperationException("Can't create new board. No rows were affected.");
            }

            _logger.LogInformation("Created new board with id '{id}'.", board.Id);

            return board.Id;
        }

        private async Task<Board> CreateBoardAsync(CreateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = _mapper.Map<Board>(request);

            await _unitOfWork.GetRepository<Board>().Add(board, false, cancellationToken);

            return board;
        }

        private async Task<BoardMember> CreateBoardMemberAsync(Guid userId, Guid boardId, string ownerNickname, CancellationToken cancellationToken)
        {
            var boardMember = new BoardMember
            {
                AccountId = userId,
                BoardId = boardId,
                Nickname = ownerNickname
            };

            await _unitOfWork.GetRepository<BoardMember>().Add(boardMember, false, cancellationToken);

            return boardMember;
        }

        private async Task AddPermissionsAsync(Guid boardMemberId, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission>().GetAllAsync(cancellationToken);
            foreach (var permission in permissions)
            {
                var boardMemberPermission = new BoardMemberPermission
                {
                    BoardMemberId = boardMemberId,
                    BoardPermissionId = permission.Id
                };

                await _unitOfWork.GetRepository<BoardMemberPermission>().Add(boardMemberPermission, false, cancellationToken);
            }
        }
    }
}
