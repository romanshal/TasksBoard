using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest
{
    public class CreateBoardInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<CreateBoardInviteRequestCommandHandler> logger) : IRequestHandler<CreateBoardInviteRequestCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<CreateBoardInviteRequestCommandHandler> _logger = logger;

        public async Task<Guid> Handle(CreateBoardInviteRequestCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var isMemberExist = board.BoardMembers.Any(member => member.AccountId == request.ToAccountId);
            if (isMemberExist)
            {
                _logger.LogInformation($"Board member with account id '{request.ToAccountId} is already exist in board '{request.BoardId}'.");
                throw new AlreadyExistException($"Board member is already exist for board '{board.Name}'.");
            }

            var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(request.BoardId, request.ToAccountId, cancellationToken);
            if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
            {
                _logger.LogInformation($"Board invite request to account id '{request.ToAccountId} is already exist in board '{request.BoardId}'.");
                throw new AlreadyExistException($"Invite request is already exist for board '{board.Name}'.");
            }

            var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(request.BoardId, request.ToAccountId, cancellationToken);
            if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
            {
                _logger.LogInformation($"Board access request to account from '{request.ToAccountId} is already exist in board '{request.BoardId}'.");
                throw new AlreadyExistException($"Access request is already exist for board '{board.Name}'.");
            }

            inviteRequest = _mapper.Map<BoardInviteRequest>(request);

            await _unitOfWork.GetRepository<BoardInviteRequest>().Add(inviteRequest, true, cancellationToken);

            if (inviteRequest.Id == Guid.Empty)
            {
                _logger.LogError($"Can't create new board invite request to board with id '{request.BoardId}'.");
                throw new ArgumentException(nameof(inviteRequest));
            }

            await _outboxService.CreateNewOutboxEvent(new NewBoardInviteRequestEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                AccountId = inviteRequest.ToAccountId,
                FromAccountId = inviteRequest.FromAccountId,
                FromAccountName = board.BoardMembers.FirstOrDefault(member => member.AccountId == inviteRequest.FromAccountId)!.Nickname
            }, cancellationToken);

            _logger.LogInformation($"Board invite request with id '{inviteRequest.Id}' added to board with id '{request.BoardId}'.");

            return inviteRequest.Id;
        }
    }
}
