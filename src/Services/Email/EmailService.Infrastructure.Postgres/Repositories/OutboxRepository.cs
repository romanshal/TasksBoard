using Dapper;
using EmailService.Core.Constants;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Infrastructure.Postgres.Constants;
using EventBus.Messages.Abstraction.Events;
using Npgsql;

namespace EmailService.Infrastructure.Postgres.Repositories
{
    internal class OutboxRepository(
        NpgsqlDataSource dataSource) : IOutboxRespository
    {
        public async Task<IReadOnlyList<EmailMessageEvent>> FetchAndClaimBatchAsync(int batchSize, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            // Claim rows using FOR UPDATE SKIP LOCKED pattern
            var messages = (await connection.QueryAsync<EmailMessageEvent>(
                SqlQueries.SqlFetchAndClaim,
                new 
                { 
                    StatusPending = (int)OutboxStatuses.Pending, 
                    BatchSize = batchSize, 
                    StatusProgress = (int)OutboxStatuses.InProgress 
                },
                transaction)).ToList();

            await transaction.CommitAsync(cancellationToken);

            return messages;
        }

        public async Task MarkSentAsync(string id, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(
                SqlQueries.SqlMarkSent,
                new 
                { 
                    Id = id, 
                    Status = (int)OutboxStatuses.Sent 
                });
        }

        public async Task MarkFailedAsync(string id, string error, int nextAttemptSeconds, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(
                SqlQueries.SqlMarkFailed,
                new
                {
                    Id = id,
                    Error = error ?? string.Empty,
                    NextSec = nextAttemptSeconds,
                    MaxAttempts = 3,
                    FailedStatus = (int)OutboxStatuses.Failed,
                    PendingStatus = (int)OutboxStatuses.Pending,
                });
        }
    }
}
