using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public class RequestBoardAccessCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<RequestBoardAccessCommandHandler> logger) : IRequestHandler<RequestBoardAccessCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<RequestBoardAccessCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(RequestBoardAccessCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var boardId = BoardId.Of(request.BoardId);

                var board = await _unitOfWork.GetBoardRepository().GetAsync(boardId, token);
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

                var isMemberExist = board.BoardMembers.Any(member => member.AccountId == request.AccountId);
                if (isMemberExist)
                {
                    _logger.LogInformation("Board member with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardMemberErrors.AlreadyExist(board.Name));
                }

                var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(boardId, request.AccountId, token);
                if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board access request with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardAccessErrors.AlreadyExist(board.Name));
                }

                var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(boardId, request.AccountId, token);
                if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
                {
                    _logger.LogInformation("Board invite request to account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.AlreadyExist(board.Name));
                }

                accessRequest = _mapper.Map<BoardAccessRequest>(request);

                _unitOfWork.GetBoardAccessRequestRepository().Add(accessRequest);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || accessRequest.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't create new board access request to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.CantCreate(board.Name));
                }

                await _outboxService.CreateNewOutboxEvent(new NewBoardAccessRequestEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = accessRequest.AccountId,
                    BoardMembersIds = [.. board.BoardMembers.Select(member => member.AccountId)]
                }, token);

                _logger.LogInformation("Board access request with id '{id}' added to board with id '{boardId}'.", accessRequest.Id, request.BoardId);

                return Result.Success(accessRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
