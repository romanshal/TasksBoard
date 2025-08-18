using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Infrastructure.Consumers
{
    public class UpdateAccountInfoEventConsumer(
        IUnitOfWork unitOfWork,
        ILogger<UpdateAccountInfoEventConsumer> logger) : IConsumer<UpdateAccountInfoEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateAccountInfoEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<UpdateAccountInfoEvent> context)
        {
            var members = await _unitOfWork.GetBoardMemberRepository().GetByAccountIdAsync(context.Message.AccountId);
            var accessRequests = await _unitOfWork.GetBoardAccessRequestRepository().GetByAccountIdAsync(context.Message.AccountId);
            var toInviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByToAccountIdAsync(context.Message.AccountId);
            var fromInviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByFromAccountIdAsync(context.Message.AccountId);
            if (!members.Any() && !accessRequests.Any() && !toInviteRequests.Any() && !fromInviteRequests.Any())
            {
                return;
            }

            foreach (var member in members)
            {
                //member.Nickname = context.Message.AccountName;

                _unitOfWork.GetBoardMemberRepository().Update(member);
            }

            foreach (var request in accessRequests)
            {
                //request.AccountName = context.Message.AccountName;
                //request.AccountEmail = context.Message.AccountEmail;

                _unitOfWork.GetBoardAccessRequestRepository().Update(request);
            }

            foreach (var request in toInviteRequests)
            {
                //request.ToAccountName = context.Message.AccountName;
                //request.ToAccountEmail = context.Message.AccountEmail;

                _unitOfWork.GetBoardInviteRequestRepository().Update(request);
            }

            foreach (var request in fromInviteRequests)
            {
                //request.FromAccountName = context.Message.AccountName;

                _unitOfWork.GetBoardInviteRequestRepository().Update(request);
            }

            var affectedRows = await _unitOfWork.SaveChangesAsync();
            if (affectedRows == 0)
            {
                _logger.LogError("Error when update information for account with id '{accountId}'.", context.Message.AccountId);
                return;
            }

            _logger.LogInformation("Information for account with id '{accountId}' successfully updated.", context.Message.AccountId);
        }
    }
}
