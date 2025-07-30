using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
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
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
            if (member is null)
            {
                _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                return Result.Failure(BoardMemberErrors.NotFound);

                //throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
            }

            member.BoardMemberPermissions.Clear();

            member.BoardMemberPermissions = [.. request.Permissions.Select(permission => new BoardMemberPermission
            {
                BoardMemberId = request.MemberId,
                BoardPermissionId = permission
            })];

            _unitOfWork.GetBoardMemberRepository().Update(member);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                _logger.LogError("Can't save new board member permissions with id '{memberId}' to board with id '{boardId}'.", request.MemberId, request.BoardId);
                return Result.Failure(BoardMemberPermissionErrors.CantCreate);

                //throw new ArgumentException($"Can't save new board member permissions with id '{request.MemberId}' to board with id '{request.BoardId}'.");
            }

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

            _logger.LogInformation("Add new permissions for board member with account id '{accountId}' on board '{boardId}'.", member.AccountId, request.BoardId);

            return Result.Success();
        }
    }
}
