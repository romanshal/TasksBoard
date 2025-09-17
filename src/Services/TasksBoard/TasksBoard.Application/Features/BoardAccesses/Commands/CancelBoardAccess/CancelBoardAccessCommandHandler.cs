using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public class CancelBoardAccessCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelBoardAccessCommandHandler> logger) : IRequestHandler<CancelBoardAccessCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CancelBoardAccessCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CancelBoardAccessCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetAsync(BoardAccessId.Of(request.RequestId), token);
                if (accessRequest is null)
                {
                    _logger.LogWarning("Board access request with id '{requestId}' was not found.", request.RequestId);
                    return Result.Failure<Guid>(BoardAccessErrors.NotFound);
                }

                accessRequest.Status = (int)BoardAccessRequestStatuses.Canceled;

                _unitOfWork.GetBoardAccessRequestRepository().Update(accessRequest);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || accessRequest.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't cancel board access request with id '{id}'.", accessRequest.Id);
                    return Result.Failure<Guid>(BoardAccessErrors.CantCancel);
                }

                //TODO: add event

                _logger.LogInformation("Board access request with id '{id}' canceled.", accessRequest.Id);

                return Result.Success(accessRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
