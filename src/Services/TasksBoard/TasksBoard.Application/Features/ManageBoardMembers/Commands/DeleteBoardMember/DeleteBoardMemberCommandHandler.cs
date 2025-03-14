using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public class DeleteBoardMemberCommandHandler(
        ILogger<DeleteBoardMemberCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardMemberCommand, Unit>
    {
        private readonly ILogger<DeleteBoardMemberCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeleteBoardMemberCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetBoardRepository().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAndUserIdAsync(request.BoardId, request.UserId, cancellationToken);
            if (member is null)
            {
                _logger.LogWarning($"Board member with id '{request.UserId}' not found in board '{board.Id}'.");
                throw new NotFoundException($"Board member with id '{request.UserId}' not found in board '{board.Name}'.");
            }

            await _unitOfWork.GetBoardMemberRepository().Delete(member, true, cancellationToken);

            return Unit.Value;
        }
    }
}
