using Common.Outbox.Abstraction.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Outbox.Services
{
    internal class OutboxPublisherBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OutboxPublisherBackgroundService> logger) : BackgroundService
    {
        private const int OutboxProcessorFrequency = 5;
        private readonly int _maxParallelism = 5;
        //private readonly int _maxParallelism = 1;
        private int _totalIterations = 0;
        private int _totalProcessedMessage = 0;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            OutboxLoggerMessages.LogStarting(logger);

            //using var cts = new CancellationTokenSource(TimeSpan.FromHours(10));
            //using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, stoppingToken);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = _maxParallelism,
                CancellationToken = stoppingToken
            };

            try
            {
                await Parallel.ForEachAsync(
                    Enumerable.Range(0, _maxParallelism),
                    parallelOptions,
                    async (_, token) =>
                    {
                        await ProcessOutboxMessages(token);
                    });
            }
            catch (OperationCanceledException)
            {
                OutboxLoggerMessages.LogOperationCancelled(logger);
            }
            catch (Exception ex)
            {
                OutboxLoggerMessages.LogError(logger, ex);
            }
            finally
            {
                OutboxLoggerMessages.LogFinished(logger, _totalIterations, _totalProcessedMessage);
            }
        }

        private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var iterationCount = Interlocked.Increment(ref _totalIterations);

                OutboxLoggerMessages.LogStartingIteration(logger, iterationCount);

                int processedMessages = await outboxProcessor.Execute(cancellationToken);
                var totalProcessedMessages = Interlocked.Add(ref _totalProcessedMessage, processedMessages);

                OutboxLoggerMessages.LogIterationCompleted(logger, iterationCount, processedMessages, totalProcessedMessages);

                // Simulate running Outbox processing every N seconds
                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), cancellationToken);
            }
        }
    }
}
