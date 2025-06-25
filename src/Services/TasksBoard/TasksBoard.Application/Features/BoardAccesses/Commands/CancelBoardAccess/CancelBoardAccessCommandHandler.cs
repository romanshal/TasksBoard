using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public class CancelBoardAccessCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelBoardAccessCommandHandler> logger) : IRequestHandler<CancelBoardAccessCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CancelBoardAccessCommandHandler> _logger = logger;

        public async Task<Guid> Handle(CancelBoardAccessCommand request, CancellationToken cancellationToken)
        {
            var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetAsync(request.RequestId, cancellationToken);
            if (accessRequest is null)
            {
                _logger.LogWarning($"Board access request with id '{request.RequestId}' not found.");
                throw new NotFoundException($"Board access request not found.");
            }

            accessRequest.Status = (int)BoardAccessRequestStatuses.Canceled;

            await _unitOfWork.GetBoardAccessRequestRepository().Update(accessRequest, true, cancellationToken);

            if (accessRequest.Id == Guid.Empty)
            {
                _logger.LogError($"Can't cancel board access request with id '{accessRequest.Id}'.");
                throw new ArgumentException(nameof(accessRequest));
            }

            _logger.LogInformation($"Board access request with id '{accessRequest.Id}' canceled.");

            return accessRequest.Id;
        }
    }
}
