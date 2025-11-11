using EmailService.Core.Interfaces;
using EmailService.Core.Interfaces.Repositories;
using EmailService.Core.Models;
using EmailService.Infrastructure.RabbitMQ.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private readonly ConcurrentQueue<EmailMessage> _buffer = new();

        private const int BatchSize = 50;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(5);

        public RabbitMqListener(IOptions<RabbitMqOptions> opts, IConnectionFactory connectionFactory, IInboxRepository outbox, ILogger<RabbitMqListener> logger)
        {
            _opts = opts.Value;
            _outbox = outbox;
            _logger = logger;
            _queue = _opts.QueueName;
            _connectionFactory = connectionFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _conn = await _connectionFactory.CreateConnectionAsync(stoppingToken);
            _channel = await _conn.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(0, _opts.PrefetchCount, false, stoppingToken);
            await _channel.QueueDeclareAsync(queue: _queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageAsync;

            await _channel.BasicConsumeAsync(_queue, autoAck: false, consumer, cancellationToken: stoppingToken);

            _logger.LogInformation("RabbitMQ listener started on queue {queue}", _queue);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(FlushInterval, stoppingToken);
                await FlushBatchAsync();
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
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
                var message = JsonSerializer.Deserialize<EmailMessage>(json, _options);

                if (message is null)
                {
                    _logger.LogWarning("Received null or invalid payload, nack requeue=false");
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
                _logger.LogError(ex, "Error processing rabbit message, nack/requeue");
                await _channel.BasicNackAsync(tag, false, true);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ listener...");
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

        private async Task FlushBatchAsync()
        {
            List<EmailMessage> batch;

            if (_buffer.IsEmpty)
                return;

            batch = [.. _buffer];
            _buffer.Clear();

            try
            {
                await _outbox.SaveBatchAsync(batch);
                _logger.LogInformation("Flushed {count} messages to DB", batch.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flushing batch to DB");
                // TODO: реализовать retry или requeue
            }
        }
    }
}
