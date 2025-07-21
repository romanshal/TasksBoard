using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public class AddBoardMemberPermissionsCommandHandler(
        ILogger<AddBoardMemberPermissionsCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IOutboxService outboxService) : IRequestHandler<AddBoardMemberPermissionsCommand, Unit>
    {
        private readonly ILogger<AddBoardMemberPermissionsCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

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

            member.BoardMemberPermissions = [.. request.Permissions.Select(permission => new BoardMemberPermission
            {
                BoardMemberId = request.MemberId,
                BoardPermissionId = permission
            })];

            await _unitOfWork.GetBoardMemberRepository().Update(member, true, cancellationToken);

            await _outboxService.CreateNewOutboxEvent(new NewBoardMemberPermissionsEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                AccountId = member.AccountId,
                AccountName = member.Nickname,
                SourceAccountId = request.AccountId,
                SourceAccountName = board.BoardMembers.First(m => m.AccountId == request.AccountId).Nickname,
                BoardMembersIds = [.. board.BoardMembers.Where(m => m.AccountId != request.AccountId).Select(m => m.AccountId)]
            }, cancellationToken);

            _logger.LogInformation($"Add new permissions for board member with account id '{member.AccountId}' on board '{request.BoardId}'.");

            return Unit.Value;
        }
    }
}
