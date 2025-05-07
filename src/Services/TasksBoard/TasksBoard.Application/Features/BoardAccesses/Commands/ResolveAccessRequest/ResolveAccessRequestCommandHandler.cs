using AutoMapper;
using AutoMapper.Execution;
using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMediator mediator,
        ILogger<ResolveAccessRequestCommandHandler> logger) : IRequestHandler<ResolveAccessRequestCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ResolveAccessRequestCommandHandler> _logger = logger;

        public async Task<Guid> Handle(ResolveAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var isBoardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!isBoardExist)
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
            }
            else
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return accessRequest.Id;
        }
    }
}
