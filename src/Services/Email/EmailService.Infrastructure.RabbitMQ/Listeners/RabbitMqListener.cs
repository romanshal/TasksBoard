using EmailService.Core.Constants;
using EmailService.Core.Interfaces;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Infrastructure.RabbitMQ.Options;
using EventBus.Messages.Abstraction.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace EmailService.Infrastructure.RabbitMQ.Listeners
{
    internal sealed class RabbitMqListener : BackgroundService, IMessageListener
    {
        private readonly RabbitMqOptions _opts;
        private readonly IInboxRepository _outbox;
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection? _conn;
        private IChannel? _channel;
        private readonly string _queue = string.Empty;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly ConcurrentQueue<EmailMessageEvent> _buffer = new();

        private const int BatchSize = 50;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(5);

        private readonly AsyncRetryPolicy _retryPolicy;

        public RabbitMqListener(IOptions<RabbitMqOptions> opts, IConnectionFactory connectionFactory, IInboxRepository outbox, ILogger<RabbitMqListener> logger)
        {
            _opts = opts.Value;
            _outbox = outbox;
            _logger = logger;
            _queue = _opts.QueueName;
            _connectionFactory = connectionFactory;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await InitializeRabbitMqAsync(stoppingToken);

                await StartConsumerAsync(stoppingToken);

                RabbitMqLoggerMessages.StartLogging(_logger, _queue);

                await RunBatchLoopAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                RabbitMqLoggerMessages.LogOperationCancelled(_logger);
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogError(_logger, ex);
                throw;
            }
        }

        private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
        {
            _conn = await _connectionFactory.CreateConnectionAsync("EmailService.API", cancellationToken);
            _channel = await _conn.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(0, _opts.PrefetchCount, false, cancellationToken);
            await _channel.ExchangeDeclareAsync(_queue, "fanout", false, false, cancellationToken: cancellationToken);
            await _channel.QueueDeclareAsync(
                queue: _queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(_queue, _queue, "", cancellationToken: cancellationToken);
        }

        private async Task StartConsumerAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += OnMessageAsync;

            await _channel!.BasicConsumeAsync(
                queue: _queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);
        }

        private async Task RunBatchLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(FlushInterval, cancellationToken);
                    await FlushBatchAsync();
                }
                catch (OperationCanceledException)
                {
                    RabbitMqLoggerMessages.LogOperationCancelled(_logger);
                    break;
                }
                catch (Exception ex)
                {
                    RabbitMqLoggerMessages.LogBatchFlushError(_logger, ex);
                }
            }
        }

        private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            if (_channel is null)
                return;

            var body = ea.Body.ToArray();
            var tag = ea.DeliveryTag;

            try
            {
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<EmailMessageEvent>(json, _options);

                if (message is null)
                {
                    RabbitMqLoggerMessages.LogNullValue(_logger);
                    await _channel.BasicNackAsync(tag, false, false);
                    return;
                }

                _buffer.Enqueue(message);

                await _channel.BasicAckAsync(tag, false);

                if (_buffer.Count >= BatchSize)
                {
                    await FlushBatchAsync();
                }
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogProccessingError(_logger, ex);
                await _channel.BasicNackAsync(tag, false, true);
            }
        }

        private async Task FlushBatchAsync()
        {
            List<EmailMessageEvent> batch;

            if (_buffer.IsEmpty)
                return;

            batch = [.. _buffer];
            _buffer.Clear();

            try
            {
                await _retryPolicy.ExecuteAsync(() => _outbox.SaveBatchAsync(batch));
                RabbitMqLoggerMessages.LogFlushSuccess(_logger, batch.Count);
            }
            catch (Exception ex)
            {
                foreach (var msg in batch)
                    _buffer.Enqueue(msg);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            RabbitMqLoggerMessages.StopLogging(_logger);

            _channel?.CloseAsync(cancellationToken);
            _conn?.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _conn?.Dispose();
            base.Dispose();
        }
    }
}
