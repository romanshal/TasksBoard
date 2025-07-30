using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest
{
    public class CreateBoardInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<CreateBoardInviteRequestCommandHandler> logger) : IRequestHandler<CreateBoardInviteRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<CreateBoardInviteRequestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateBoardInviteRequestCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<Guid>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var isMemberExist = board.BoardMembers.Any(member => member.AccountId == request.ToAccountId);
            if (isMemberExist)
            {
                _logger.LogInformation("Board member with account id '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                return Result.Failure<Guid>(BoardMemberErrors.AlreadyExist(board.Name));

                //throw new AlreadyExistException($"Board member is already exist for board '{board.Name}'.");
            }

            var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(request.BoardId, request.ToAccountId, cancellationToken);
            if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
            {
                _logger.LogInformation("Board invite request to account id '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                return Result.Failure<Guid>(BoardInviteErrors.AlreadyExist(board.Name));

                //throw new AlreadyExistException($"Invite request is already exist for board '{board.Name}'.");
            }

            var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(request.BoardId, request.ToAccountId, cancellationToken);
            if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
            {
                _logger.LogInformation("Board access request to account from '{toAccountId} is already exist in board '{boardId}'.", request.ToAccountId, request.BoardId);
                return Result.Failure<Guid>(BoardAccessErrors.AlreadyExist(board.Name));

                //throw new AlreadyExistException($"Access request is already exist for board '{board.Name}'.");
            }

            inviteRequest = _mapper.Map<BoardInviteRequest>(request);

            _unitOfWork.GetRepository<BoardInviteRequest>().Add(inviteRequest);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || inviteRequest.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board invite request to board with id '{boardId}'.", request.BoardId);
                return Result.Failure<Guid>(BoardInviteErrors.CantCreate(board.Name));

                //throw new ArgumentException(nameof(inviteRequest));
            }

            await _outboxService.CreateNewOutboxEvent(new NewBoardInviteRequestEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                AccountId = inviteRequest.ToAccountId,
                FromAccountId = inviteRequest.FromAccountId,
                FromAccountName = board.BoardMembers.FirstOrDefault(member => member.AccountId == inviteRequest.FromAccountId)!.Nickname
            }, cancellationToken);

            _logger.LogInformation("Board invite request with id '{id}' added to board with id '{boardId}'.", inviteRequest.Id, request.BoardId);

            return Result.Success(inviteRequest.Id);
        }
    }
}
