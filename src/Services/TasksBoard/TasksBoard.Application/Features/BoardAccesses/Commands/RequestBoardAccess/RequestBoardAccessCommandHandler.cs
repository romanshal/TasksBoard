﻿using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public class RequestBoardAccessCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<RequestBoardAccessCommandHandler> logger) : IRequestHandler<RequestBoardAccessCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<RequestBoardAccessCommandHandler> _logger = logger;

        public async Task<Guid> Handle(RequestBoardAccessCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetBoardRepository().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            if (!board.Public)
            {
                _logger.LogWarning("Access request to private board '{id} from account with id '{accountId}'.", board.Id, request.AccountId);
                throw new ForbiddenException($"Board '{board.Name} is private.'");
            }

            var isMemberExist = board.BoardMembers.Any(member => member.AccountId == request.AccountId);
            if (isMemberExist)
            {
                _logger.LogInformation("Board member with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                throw new AlreadyExistException($"Board member is already exist for board '{board.Name}'.");
            }

            var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(request.BoardId, request.AccountId, cancellationToken);
            if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
            {
                _logger.LogInformation("Board access request with account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                throw new AlreadyExistException($"Access request is already exist for board '{board.Name}'.");
            }

            var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetByBoardIdAndToAccountIdAsync(request.BoardId, request.AccountId, cancellationToken);
            if (inviteRequest is not null && inviteRequest.Status == (int)BoardInviteRequestStatuses.Pending)
            {
                _logger.LogInformation("Board invite request to account id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                throw new AlreadyExistException($"Invite request is already exist for board '{board.Name}'.");
            }

            accessRequest = _mapper.Map<BoardAccessRequest>(request);

            _unitOfWork.GetBoardAccessRequestRepository().Add(accessRequest);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || accessRequest.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board access request to board with id '{boardId}'.", request.BoardId);
                throw new ArgumentException(nameof(accessRequest));
            }

            await _outboxService.CreateNewOutboxEvent(new NewBoardAccessRequestEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                AccountId = accessRequest.AccountId,
                AccountName = accessRequest.AccountName,
                BoardMembersIds = [.. board.BoardMembers.Select(member => member.AccountId)]
            }, cancellationToken);

            _logger.LogInformation("Board access request with id '{id}' added to board with id '{boardId}'.", accessRequest.Id, request.BoardId);

            return accessRequest.Id;
        }
    }
}
