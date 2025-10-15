using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Interfaces.Factories;
using Common.Outbox.Extensions;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public class RequestBoardAccessCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxEventFactory outboxFactory,
        ILogger<RequestBoardAccessCommandHandler> logger) : IRequestHandler<RequestBoardAccessCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;
        private readonly ILogger<RequestBoardAccessCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(RequestBoardAccessCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var boardId = BoardId.Of(request.BoardId);
                var accountId = AccountId.Of(request.AccountId);

                var board = await _unitOfWork.GetBoardRepository().GetAsync(boardId, noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                if (!board.Public)
                {
                    _logger.LogWarning("Access request to private board '{id} from account with id '{accountId}'.", request.BoardId, request.AccountId);
                    return Result.Failure<Guid>(BoardErrors.Private(board.Name));
                }

                var isMemberExist = board.BoardMembers.Any(member => member.AccountId == accountId);
                if (isMemberExist)
                {
                    _logger.LogInformation("Board member with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardMemberErrors.AlreadyExist(board.Name));
                }

                var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(boardId, accountId, token);
                if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board access request with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardAccessErrors.AlreadyExist(board.Name));
                }

                var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(boardId, accountId, token);
                if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board invite request to account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.AlreadyExist(board.Name));
                }

                accessRequest = _mapper.Map<BoardAccessRequest>(request);

                _unitOfWork.GetBoardAccessRequestRepository().Add(accessRequest);

                var outboxEvent = _outboxFactory.Create(new NewBoardAccessRequestEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = accessRequest.AccountId.Value,
                    UsersInterested = [.. board.BoardMembers.Select(member => member.AccountId.Value)]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || accessRequest.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't create new board access request to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.CantCreate(board.Name));
                }

                _logger.LogInformation("Board access request with id '{id}' added to board with id '{boardId}'.", accessRequest.Id, request.BoardId);

                return Result.Success(accessRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
