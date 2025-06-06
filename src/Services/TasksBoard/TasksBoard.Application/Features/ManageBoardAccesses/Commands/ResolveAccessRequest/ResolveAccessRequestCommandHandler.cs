using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxService outboxService,
        IMediator mediator,
        ILogger<ResolveAccessRequestCommandHandler> logger) : IRequestHandler<ResolveAccessRequestCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<ResolveAccessRequestCommandHandler> _logger = logger;

        public async Task<Guid> Handle(ResolveAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var accessRequest = await _unitOfWork.GetRepository<BoardAccessRequest>().GetAsync(request.RequestId, cancellationToken);
            if (accessRequest is null)
            {
                _logger.LogWarning($"Board access request with id '{request.RequestId}' not found.");
                throw new NotFoundException($"Board access request with id '{request.RequestId}' not found.");
            }

            accessRequest.Status = request.Decision ? (int)BoardAccessRequestStatuses.Accepted : (int)BoardAccessRequestStatuses.Rejected;

            await _unitOfWork.GetRepository<BoardAccessRequest>().Update(accessRequest, false, cancellationToken);

            if (request.Decision)
            {
                var result = await _mediator.Send(new AddBoardMemberCommand
                {
                    BoardId = accessRequest.BoardId,
                    AccountId = accessRequest.AccountId,
                    Nickname = accessRequest.AccountName
                }, cancellationToken);

                if (result == Guid.Empty)
                {
                    _logger.LogError("Can't add new board member.");
                    throw new ArgumentException(nameof(accessRequest));
                }

                await _outboxService.CreateNewOutboxEvent(new NewBoardMemberEvent
                {
                    BoardId = board.Id,
                    BoardName = board.Name,
                    AccountId = accessRequest.AccountId,
                    AccountName = accessRequest.AccountName,
                    BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != accessRequest.AccountId).Select(member => member.AccountId)]
                }, cancellationToken);
            }
            else
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _outboxService.CreateNewOutboxEvent(new ResolveAccessRequestEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                AccountId = accessRequest.AccountId,
                SourceAccountId = request.ResolveUserId,
                SourceAccountName = board.BoardMembers.FirstOrDefault(member => member.AccountId == request.ResolveUserId)!.Nickname,
                Status = request.Decision
            }, cancellationToken);

            return accessRequest.Id;
        }
    }
}
