using Dapper;
using EmailService.Core.Constants;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Core.Models;
using Npgsql;

namespace EmailService.Infrastructure.Postgres.Repositories
{
    internal class OutboxRepository(
        NpgsqlDataSource dataSource) : IOutboxRespository
    {
        public async Task<IReadOnlyList<EmailMessage>> FetchAndClaimBatchAsync(int batchSize, string workerId, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            // Claim rows using FOR UPDATE SKIP LOCKED pattern
            var sql = @"
                WITH cte AS (
                  SELECT id FROM email_outbox
                  WHERE status = @Status AND next_attempt_at <= now()
                  ORDER BY next_attempt_at
                  FOR UPDATE SKIP LOCKED
                  LIMIT @BatchSize
                )
                UPDATE email_outbox
                SET claimed_by = @WorkerId, claimed_at = now(), status = 'InProgress'
                WHERE id IN (SELECT id FROM cte)
                RETURNING id, message_id as MessageId, ""to"" as To, ""from"" as From,
                          subject as Subject, body as Body, is_html as IsHtml,
                          created_at as CreatedAt, attempts as Attempts,
                          next_attempt_at as NextAttemptAt, status as Status, last_error as LastError;
            ";

            var messages = (await connection.QueryAsync<EmailMessage>(
                sql,
                new { Status = (int)OutboxStatuses.Pending, BatchSize = batchSize, WorkerId = workerId },
                transaction)).ToList();

            await transaction.CommitAsync(cancellationToken);

            return messages;
        }

        public async Task MarkSentAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            const string sql = @"UPDATE email_outbox SET status = 'Sent', last_error = NULL WHERE id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task MarkFailedAsync(Guid id, string error, int nextAttemptSeconds, CancellationToken cancellationToken = default)
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            const string sql = @"
                UPDATE email_outbox
                SET attempts = attempts + 1,
                    last_error = @Error,
                    next_attempt_at = now() + make_interval(secs => @NextSec),
                    status = CASE WHEN attempts + 1 >= @MaxAttempts THEN @FailedStatus ELSE @PendingStatus END
                WHERE id = @Id;
            ";

            await connection.ExecuteAsync(
                sql,
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
