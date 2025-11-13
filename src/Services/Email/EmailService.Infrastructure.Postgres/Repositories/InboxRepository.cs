using Dapper;
using EmailService.Core.Constants;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Infrastructure.Postgres.Constants;
using EventBus.Messages.Abstraction.Events;
using Npgsql;
using System.Text;

namespace EmailService.Infrastructure.Postgres.Repositories
{
    internal class InboxRepository(
        NpgsqlDataSource dataSource) : IInboxRepository
    {
        public async Task SaveBatchAsync(IList<EmailMessageEvent> messages, CancellationToken cancellationToken = default)
        {
            if (messages is null || !messages.Any())
                return;

            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var sb = new StringBuilder();
            var parameters = new DynamicParameters();

            int i = 0;
            foreach (var m in messages)
            {
                if (i > 0)
                    sb.Append(',');

                sb.Append($"(@MessageId{i}, " +
                    $"@Recipient{i}, " +
                    $"@Sender{i}, " +
                    $"@Subject{i}, " +
                    $"@Body{i}, " +
                    $"@IsHtml{i}, " +
                    $"@CreatedAt{i}, " +
                    $"@Attempts{i}, " +
                    $"@NextAttemptAt{i}, " +
                    $"@Status{i})");

                parameters.Add($"MessageId{i}", m.MessageId);
                parameters.Add($"Recipient{i}", m.Recipient);
                parameters.Add($"Sender{i}", m.Sender);
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

            var formattedSql = string.Format(SqlQueries.SqlInsert, valuesList);

            await connection.ExecuteAsync(formattedSql, parameters, transaction);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
