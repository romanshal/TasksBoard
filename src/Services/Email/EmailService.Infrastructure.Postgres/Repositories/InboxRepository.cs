using Dapper;
using EmailService.Core.Constants;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Core.Models;
using Npgsql;
using System.Text;

namespace EmailService.Infrastructure.Postgres.Repositories
{
    internal class InboxRepository(
        NpgsqlDataSource dataSource) : IInboxRepository
    {
        public async Task SaveBatchAsync(IList<EmailMessage> messages, CancellationToken cancellationToken = default)
        {
            if (messages is null || !messages.Any())
                return;

            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            const string sql = @"
                INSERT INTO email_outbox(
                    message_id, 
                    ""to"", 
                    ""from"", 
                    subject, 
                    body, 
                    is_html, 
                    created_at, 
                    attempts, 
                    next_attempt_at, 
                    status
                )
                VALUES {0}
                ON CONFLICT (message_id) DO NOTHING;
                ";

            var sb = new StringBuilder();
            var parameters = new DynamicParameters();

            int i = 0;
            foreach (var m in messages)
            {
                if (i > 0)
                    sb.Append(',');

                sb.Append($"(@MessageId{i}, @To{i}, @From{i}, @Subject{i}, @Body{i}, @IsHtml{i}, @CreatedAt{i}, @Attempts{i}, @NextAttemptAt{i}, @Status{i})");

                parameters.Add($"MessageId{i}", m.MessageId);
                parameters.Add($"To{i}", m.To);
                parameters.Add($"From{i}", m.From);
                parameters.Add($"Subject{i}", m.Subject);
                parameters.Add($"Body{i}", m.Body);
                parameters.Add($"IsHtml{i}", m.IsHtml);
                parameters.Add($"CreatedAt{i}", DateTimeOffset.UtcNow);
                parameters.Add($"Attempts{i}", 0);
                parameters.Add($"NextAttemptAt{i}", DateTimeOffset.UtcNow);
                parameters.Add($"Status{i}", (int)OutboxStatuses.Pending);

                i++;
            }

            var valuesList = sb.ToString();

            var formattedSql = string.Format(sql, valuesList);

            await connection.ExecuteAsync(formattedSql, parameters, transaction);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
