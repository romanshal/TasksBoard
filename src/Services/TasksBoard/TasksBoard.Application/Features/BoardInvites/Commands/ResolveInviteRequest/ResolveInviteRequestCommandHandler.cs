using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public class ResolveInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxService outboxService,
        IMediator mediator,
        ILogger<ResolveInviteRequestCommandHandler> logger) : IRequestHandler<ResolveInviteRequestCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<ResolveInviteRequestCommandHandler> _logger = logger;

        public async Task<Guid> Handle(ResolveInviteRequestCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var inviteRequest = await _unitOfWork.GetRepository<BoardInviteRequest>().GetAsync(request.RequestId, cancellationToken);
            if (inviteRequest is null)
            {
                _logger.LogWarning($"Board access request with id '{request.RequestId}' not found.");
                throw new NotFoundException($"Board access request with id '{request.RequestId}' not found.");
            }

            inviteRequest.Status = request.Decision ? (int)BoardInviteRequestStatuses.Accepted : (int)BoardInviteRequestStatuses.Rejected;

            await _unitOfWork.GetRepository<BoardInviteRequest>().Update(inviteRequest, false, cancellationToken);

            if (request.Decision)
            {
                var result = await _mediator.Send(new AddBoardMemberCommand
                {
                    BoardId = inviteRequest.BoardId,
                    AccountId = inviteRequest.ToAccountId,
                    Nickname = inviteRequest.ToAccountName
                }, cancellationToken);

                if (result == Guid.Empty)
                {
                    _logger.LogError("Can't add new board member.");
                    throw new ArgumentException(nameof(inviteRequest));
                }

                await _outboxService.CreateNewOutboxEvent(new NewBoardMemberEvent
                {
                    BoardId = board.Id,
                    BoardName = board.Name,
                    AccountId = inviteRequest.ToAccountId,
                    AccountName = inviteRequest.ToAccountName,
                    BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != inviteRequest.ToAccountId).Select(member => member.AccountId)]
                }, cancellationToken);
            }
            else
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return inviteRequest.Id;
        }
    }
}
