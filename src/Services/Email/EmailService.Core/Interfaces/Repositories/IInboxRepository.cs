using EmailService.Core.Models;

namespace EmailService.Core.Interfaces.Repositories
{
    public interface IInboxRepository
    {
        Task SaveBatchAsync(IList<EmailMessage> messages, CancellationToken cancellationToken = default);
    }
}
