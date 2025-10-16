using Common.Outbox.Abstraction.Constants;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.ValueObjects;
using Dapper;
using EventBus.Messages.Abstraction.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace Common.Outbox.Services
{
    internal sealed class OutboxProcessor(
        NpgsqlDataSource dataSource,
        IPublishEndpoint publishEndpoint,
        ILogger<OutboxProcessor> logger)
    {
        private const int BatchSize = 1000;
        private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

        public async Task<int> Execute(CancellationToken cancellationToken = default)
        {
            var totalStopwatch = Stopwatch.StartNew();
            var stepStopwatch = new Stopwatch();

            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            stepStopwatch.Restart();

            var messages = (await connection.QueryAsync<OutboxEvent>(
                """
                SELECT 
                    "Id",
                    "CreatedAt",
                    "LastModifiedAt",
                    "EventType",
                    "Payload",
                    "Status"
                FROM outboxevents
                WHERE "Status" = @Status
                ORDER BY "CreatedAt"
                LIMIT @BatchSize
                FOR UPDATE SKIP LOCKED
                """,
                param: new { Status = OutboxEventStatuses.Created, BatchSize },
                transaction: transaction))
                .AsList();

            var queryTime = stepStopwatch.ElapsedMilliseconds;

            var updateQueue = new ConcurrentQueue<OutboxUpdate>();

            var publishTasks = messages
                .Select(message => PublishMessage(message, updateQueue, publishEndpoint, cancellationToken))
                .ToList();

            await Task.WhenAll(publishTasks);
            var publishTime = stepStopwatch.ElapsedMilliseconds;

            stepStopwatch.Restart();
            if (!updateQueue.IsEmpty)
            {
                var updateSql =
                    """
                    UPDATE outboxevents
                    SET "LastModifiedAt" = v.LastModifiedAt, "Status" = v.Status
                    FROM (VALUES {0}) AS v(Id, LastModifiedAt, Status)
                    WHERE "Id" = v.Id::uuid
                    """;

                var updates = updateQueue.ToList();

                var valuesList = string.Join(",", updateQueue.Select((_, i) => $"(@Id{i}, @LastModifiedAt{i}, @Status{i})"));

                var parameters = new DynamicParameters();

                for (int i = 0; i < updateQueue.Count; i++)
                {
                    parameters.Add($"Id{i}", updates[i].Id.Value.ToString());
                    parameters.Add($"LastModifiedAt{i}", updates[i].LastModifiedAt);
                    parameters.Add($"Status{i}", updates[i].Status);
                }

                var formattedSql = string.Format(updateSql, valuesList);

                await connection.ExecuteAsync(formattedSql, parameters, transaction: transaction);
            }

            var updateTime = stepStopwatch.ElapsedMilliseconds;

            await transaction.CommitAsync(cancellationToken);

            totalStopwatch.Stop();
            var totalTime = totalStopwatch.ElapsedMilliseconds;

            OutboxLoggerMessages.LogProcessingPerformance(logger, totalTime, queryTime, publishTime, updateTime, messages.Count);

            return messages.Count;
        }

        private static async Task PublishMessage(
            OutboxEvent outboxEvent,
            ConcurrentQueue<OutboxUpdate> updateQueue,
            IPublishEndpoint publishEndpoint,
            CancellationToken cancellationToken)
        {
            var messageType = GetOrAddMessageType(outboxEvent.EventType);
            var deserializedMessage = JsonSerializer.Deserialize(outboxEvent.Payload, messageType)!;

            await publishEndpoint.Publish(deserializedMessage, cancellationToken);

            updateQueue.Enqueue(new OutboxUpdate 
            { 
                Id = outboxEvent.Id,
                LastModifiedAt = DateTime.UtcNow,
                Status = OutboxEventStatuses.Sent
            });
        }

        private static Type GetOrAddMessageType(string typename)
        {
            var assembly = typeof(BaseEvent).Assembly;
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == typename);

            return TypeCache.GetOrAdd(typename, name => type!);
        }

        private readonly struct OutboxUpdate
        {
            public OutboxId Id { get; init; }
            public DateTime LastModifiedAt { get; init; }
            public string Status { get; init; }
        }
    }
}
