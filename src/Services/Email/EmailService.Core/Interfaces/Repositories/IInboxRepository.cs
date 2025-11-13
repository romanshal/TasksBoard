using EventBus.Messages.Abstraction.Events;

namespace EmailService.Core.Interfaces.Repositories
{
    public interface IInboxRepository
    {
        Task SaveBatchAsync(IList<EmailMessageEvent> messages, CancellationToken cancellationToken = default);
    }
}
