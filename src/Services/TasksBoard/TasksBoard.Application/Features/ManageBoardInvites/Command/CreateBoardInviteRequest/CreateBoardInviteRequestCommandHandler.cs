using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Extensions;
using Common.Outbox.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest
{
    public class CreateBoardInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxEventFactory outboxFactory,
        ILogger<CreateBoardInviteRequestCommandHandler> logger) : IRequestHandler<CreateBoardInviteRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;
        private readonly ILogger<CreateBoardInviteRequestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateBoardInviteRequestCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var boardId = BoardId.Of(request.BoardId);
                var toAccountId = AccountId.Of(request.ToAccountId);

                var board = await _unitOfWork.GetBoardRepository().GetAsync(boardId, noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var isMemberExist = board.BoardMembers.Any(member => member.AccountId == toAccountId);
                if (isMemberExist)
                {
                    _logger.LogInformation("Board member with account id '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardMemberErrors.AlreadyExist(board.Name));
                }

                var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(boardId, toAccountId, token);
                if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board invite request to account id '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.AlreadyExist(board.Name));
                }

                var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(boardId, toAccountId, token);
                if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board access request to account from '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardAccessErrors.AlreadyExist(board.Name));
                }

                inviteRequest = _mapper.Map<BoardInviteRequest>(request);

                _unitOfWork.GetBoardInviteRequestRepository().Add(inviteRequest);

                var outboxEvent = _outboxFactory.Create(new NewBoardInviteRequestEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = inviteRequest.ToAccountId.Value,
                    FromAccountId = inviteRequest.FromAccountId.Value,
                    UsersInterested = [inviteRequest.ToAccountId.Value]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || inviteRequest.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't create new board invite request to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.CantCreate(board.Name));
                }

                _logger.LogInformation("Board invite request with id '{id}' added to board with id '{boardId}'.", inviteRequest.Id, request.BoardId);

                return Result.Success(inviteRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
