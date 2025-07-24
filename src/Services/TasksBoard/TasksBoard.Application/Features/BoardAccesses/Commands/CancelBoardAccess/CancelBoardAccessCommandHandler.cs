using Common.Blocks.Constants;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;

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
                _logger.LogWarning("Board access request with id '{requestId}' not found.", request.RequestId);
                throw new NotFoundException($"Board access request not found.");
            }

            accessRequest.Status = (int)BoardAccessRequestStatuses.Canceled;

            _unitOfWork.GetBoardAccessRequestRepository().Update(accessRequest);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || accessRequest.Id == Guid.Empty)
            {
                _logger.LogError("Can't cancel board access request with id '{id}'.", accessRequest.Id);
                throw new ArgumentException(nameof(accessRequest));
            }

            _logger.LogInformation("Board access request with id '{id}' canceled.", accessRequest.Id);

            return accessRequest.Id;
        }
    }
}
