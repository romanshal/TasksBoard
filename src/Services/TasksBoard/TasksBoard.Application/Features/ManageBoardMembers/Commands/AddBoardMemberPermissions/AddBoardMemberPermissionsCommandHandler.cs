using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMemberPermissions
{
    public class AddBoardMemberPermissionsCommandHandler(
        ILogger<AddBoardMemberPermissionsCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IOutboxService outboxService) : IRequestHandler<AddBoardMemberPermissionsCommand, Result>
    {
        private readonly ILogger<AddBoardMemberPermissionsCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Result> Handle(AddBoardMemberPermissionsCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure(BoardErrors.NotFound);
                }

                var member = board.BoardMembers.FirstOrDefault(member => member.Id.Value == request.MemberId);
                if (member is null)
                {
                    _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberErrors.NotFound);
                }

                member.BoardMemberPermissions.Clear();

                member.BoardMemberPermissions = [.. request.Permissions.Select(permission => new BoardMemberPermission
                {
                    BoardMemberId = member.Id,
                    BoardPermissionId = BoardPermissionId.Of(permission)
                })];

                _unitOfWork.GetBoardMemberRepository().Update(member);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't save new board member permissions with id '{memberId}' to board with id '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberPermissionErrors.CantCreate);
                }

                await _outboxService.CreateNewOutboxEvent(new NewBoardMemberPermissionsEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = member.AccountId,
                    SourceAccountId = request.AccountId,
                    BoardMembersIds = [.. board.BoardMembers.Where(m => m.AccountId != request.AccountId).Select(m => m.AccountId)]
                }, token);

                _logger.LogInformation("Add new permissions for board member with account id '{accountId}' on board '{boardId}'.", member.AccountId, request.BoardId);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
