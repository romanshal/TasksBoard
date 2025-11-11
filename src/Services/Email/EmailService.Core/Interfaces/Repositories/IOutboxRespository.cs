using EmailService.Core.Models;

namespace EmailService.Core.Interfaces.Repositories
{
    public interface IOutboxRespository
    {
        Task<IReadOnlyList<EmailMessage>> FetchAndClaimBatchAsync(int batchSize, string workerId, CancellationToken cancellationToken = default);
        Task MarkSentAsync(Guid id, CancellationToken cancellationToken = default);
        Task MarkFailedAsync(Guid id, string error, int nextAttemptSeconds, CancellationToken cancellationToken = default);
    }
}
