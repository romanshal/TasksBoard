using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.Boards.Commands.CreateBoard
{
    public class CreateBoardCommandHandler(
        ILogger<CreateBoardCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateBoardCommand, Result<Guid>>
    {
        private readonly ILogger<CreateBoardCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<Guid>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = CreateBoard(request);

            var boardMember = CreateBoardMember(board.OwnerId, board.Id.Value, request.OwnerNickname);

            await AddPermissionsAsync(boardMember.Id.Value, cancellationToken);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || board.Id.Value == Guid.Empty)
            {
                _logger.LogError("Can't create new board. No rows were affected.");
                return Result.Failure<Guid>(BoardErrors.CantCreate);
            }

            _logger.LogInformation("Created new board with id '{id}'.", board.Id);

            return Result.Success(board.Id.Value);
        }

        private Board CreateBoard(CreateBoardCommand request)
        {
            var board = _mapper.Map<Board>(request);

            _unitOfWork.GetRepository<Board, BoardId>().Add(board);

            return board;
        }

        private BoardMember CreateBoardMember(Guid userId, Guid boardId, string ownerNickname)
        {
            var boardMember = new BoardMember
            {
                AccountId = userId,
                BoardId = BoardId.Of(boardId)
            };

            _unitOfWork.GetRepository<BoardMember, BoardMemberId>().Add(boardMember);

            return boardMember;
        }

        private async Task AddPermissionsAsync(Guid boardMemberId, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission, BoardPermissionId>().GetAllAsync(cancellationToken);
            foreach (var permission in permissions)
            {
                var boardMemberPermission = new BoardMemberPermission
                {
                    BoardMemberId = BoardMemberId.Of(boardMemberId),
                    BoardPermissionId = permission.Id
                };

                _unitOfWork.GetRepository<BoardMemberPermission, MemberPermissionId>().Add(boardMemberPermission);
            }
        }
    }
}
