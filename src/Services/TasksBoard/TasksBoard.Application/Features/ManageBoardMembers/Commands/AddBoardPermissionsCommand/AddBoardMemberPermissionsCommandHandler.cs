using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public class AddBoardMemberPermissionsCommandHandler(
        ILogger<AddBoardMemberPermissionsCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<AddBoardMemberPermissionsCommand, Unit>
    {
        private readonly ILogger<AddBoardMemberPermissionsCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(AddBoardMemberPermissionsCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
            if (member is null)
            {
                _logger.LogWarning($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
                throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
            }

            member.BoardMemberPermissions.Clear();

            member.BoardMemberPermissions = [.. request.Permissions.Select(permission => new Domain.Entities.BoardMemberPermission
            {
                BoardMemberId = request.MemberId,
                BoardPermissionId = permission
            })];

            await _unitOfWork.GetBoardMemberRepository().Update(member, true, cancellationToken);

            _logger.LogInformation($"Add new permissions for board member with account id '{member.AccountId}' on board '{request.BoardId}'.");

            return Unit.Value;
        }
    }
}
