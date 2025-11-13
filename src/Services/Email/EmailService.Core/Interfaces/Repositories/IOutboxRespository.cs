using EventBus.Messages.Abstraction.Events;

namespace EmailService.Core.Interfaces.Repositories
{
    public interface IOutboxRespository
    {
        Task<IReadOnlyList<EmailMessageEvent>> FetchAndClaimBatchAsync(int batchSize, CancellationToken cancellationToken = default);
        Task MarkSentAsync(string id, CancellationToken cancellationToken = default);
        Task MarkFailedAsync(string id, string error, int nextAttemptSeconds, CancellationToken cancellationToken = default);
    }
}
