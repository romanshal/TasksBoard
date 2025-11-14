using EmailService.Core.Constants;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Infrastructure.RabbitMQ.Options;
using EventBus.Messages.Abstraction.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;

namespace EmailService.Infrastructure.RabbitMQ.Proccessors
{
    internal class BatchProcessor : IDisposable, IAsyncDisposable
    {
        private readonly IInboxRepository _outbox;
        private readonly ConcurrentQueue<EmailMessageEvent> _buffer = new();
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<BatchProcessor> _logger;

        private readonly TimeSpan _flushInterval;
        private readonly int _batchSize;

        private Task? _loopTask;
        private CancellationTokenSource? _cts;

        public int Count => _buffer.Count;

        public BatchProcessor(IOptions<BatchOptions> options, IInboxRepository outbox, ILogger<BatchProcessor> logger)
        {
            _outbox = outbox;
            _logger = logger;

            var opt = options.Value;
            _flushInterval = TimeSpan.FromMilliseconds(opt.FlushIntervalMs);
            _batchSize = opt.BatchSize;

            _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt - 1)),
                onRetry: (exception, timespan, attempt, context) =>
                {
                    RabbitMqLoggerMessages.LogRetryAttempt(_logger, exception, attempt, timespan);
                });
        }

        public void Initialize(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _loopTask = Task.Run(() => RunLoopAsync(_cts.Token), _cts.Token);
        }

        public async Task Enqueue(EmailMessageEvent message, CancellationToken cancellationToken = default)
        {
            _buffer.Enqueue(message);

            if (Count >= _batchSize)
            {
                await FlushAsync(cancellationToken);
            }
        }

        private async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_buffer.IsEmpty)
                return;

            List<EmailMessageEvent> batch;

            batch = [.. _buffer];
            _buffer.Clear();

            try
            {
                await _retryPolicy.ExecuteAsync(() => _outbox.SaveBatchAsync(batch, cancellationToken));
                RabbitMqLoggerMessages.LogFlushSuccess(_logger, batch.Count);
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);

                //TODO: add retry limiter
                foreach (var msg in batch)
                    _buffer.Enqueue(msg);
            }
        }

        private async Task RunLoopAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_flushInterval, cancellationToken);
                    await FlushAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);
                }
            }

            try
            {
                await FlushAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            try
            {
                _loopTask?.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);
            }
            finally
            {
                _cts?.Dispose();
                _loopTask?.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts?.Cancel();
            if (_loopTask != null)
            {
                try
                {
                    await _loopTask;
                }
                catch (Exception ex)
                {
                    RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);
                }
            }

            _cts?.Dispose();
            _loopTask?.Dispose();
        }
    }
}
