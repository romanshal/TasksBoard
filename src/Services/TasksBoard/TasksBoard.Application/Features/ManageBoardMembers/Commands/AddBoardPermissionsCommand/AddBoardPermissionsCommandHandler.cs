using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public class AddBoardPermissionsCommandHandler(
        ILogger<AddBoardPermissionsCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<AddBoardPermissionsCommand, Unit>
    {
        private readonly ILogger<AddBoardPermissionsCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(AddBoardPermissionsCommand request, CancellationToken cancellationToken)
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

            member.BoardMemberPermissions = [.. request.Permissions.Select(permission => new BoardMemberPermission
            {
                BoardMemberId = request.UserId,
                BoardPermissionId = permission
            })];

            await _unitOfWork.GetBoardMemberRepository().Update(member, true, cancellationToken);

            _logger.LogInformation($"Add new permissions for user '{request.UserId}' on board '{request.BoardId}'.");

            return Unit.Value;
        }
    }
}
