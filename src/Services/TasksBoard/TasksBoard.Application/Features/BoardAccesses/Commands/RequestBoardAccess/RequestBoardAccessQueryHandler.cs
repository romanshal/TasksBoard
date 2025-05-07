using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public class RequestBoardAccessQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RequestBoardAccessQueryHandler> logger) : IRequestHandler<RequestBoardAccessQuery, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<RequestBoardAccessQueryHandler> _logger = logger;

        public async Task<Guid> Handle(RequestBoardAccessQuery request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            if (!board.Public)
            {
                _logger.LogWarning($"Access request to private board '{board.Id} from account with id '{request.AccountId}'.");
                throw new ForbiddenException($"Board '{board.Name} is private.'");
            }

            var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetByBoardIdAndAccountId(request.BoardId, request.AccountId, cancellationToken);
            if (accessRequest is not null && accessRequest.Status == (int)BoardAccessRequestStatuses.Pending)
            {
                _logger.LogInformation($"Board access request with account id '{request.AccountId} is already exist in board '{request.BoardId}'.");
                throw new AlreadyExistException($"Access request is already exist for board '{board.Name}'.");
            }

            accessRequest = _mapper.Map<BoardAccessRequest>(request);

            await _unitOfWork.GetRepository<BoardAccessRequest>().Add(accessRequest, true, cancellationToken);

            if (accessRequest.Id == Guid.Empty)
            {
                _logger.LogError($"Can't create new board access request to board with id '{request.BoardId}'.");
                throw new ArgumentException(nameof(accessRequest));
            }

            _logger.LogInformation($"Board access request with id '{accessRequest.Id}' added to board with id '{request.BoardId}'.");

            return accessRequest.Id;
        }
    }
}
