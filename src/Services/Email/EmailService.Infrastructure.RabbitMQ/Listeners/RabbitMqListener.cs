using EmailService.Core.Constants;
using EmailService.Core.Interfaces;
using EmailService.Infrastructure.RabbitMQ.Proccessors;
using EventBus.Messages.Abstraction.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EmailService.Infrastructure.RabbitMQ.Listeners
{
    internal sealed class RabbitMqListener(
        RabbitMqConnectionManager connectionManager,
        BatchProcessor batchProcessor,
        ILogger<RabbitMqListener> logger) : BackgroundService, IMessageListener
    {
        private readonly RabbitMqConnectionManager _connectionManager = connectionManager;
        private readonly BatchProcessor _batchProcessor = batchProcessor;
        private readonly ILogger<RabbitMqListener> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _connectionManager.InitializeAsync(stoppingToken);
                _batchProcessor.Initialize(stoppingToken);

                await StartConsumerAsync(stoppingToken);

                RabbitMqLoggerMessages.StartLogging(_logger, _connectionManager.QueueName);
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

        private async Task StartConsumerAsync(CancellationToken cancellationToken = default)
        {
            var channel = _connectionManager.Channel!;
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += OnMessageAsync;

            await channel!.BasicConsumeAsync(
                queue: _connectionManager.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);
        }

        private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            var channel = _connectionManager.Channel!;
            var body = ea.Body.ToArray();
            var tag = ea.DeliveryTag;

            try
            {
                var json = Encoding.UTF8.GetString(body);

                using var doc = JsonDocument.Parse(json);
                var messageElement = doc.RootElement.GetProperty("message");

                var message = messageElement.Deserialize<EmailMessageEvent>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (message is null)
                {
                    RabbitMqLoggerMessages.LogNullValue(_logger);
                    await channel.BasicNackAsync(tag, false, false);
                    return;
                }

                await _batchProcessor.Enqueue(message);

                await channel.BasicAckAsync(tag, false);
            }
            catch (Exception ex)
            {
                RabbitMqLoggerMessages.LogProccessingError(_logger, ex);
                await channel.BasicNackAsync(tag, false, true);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            RabbitMqLoggerMessages.StopLogging(_logger);

            await _connectionManager.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _connectionManager.Dispose();
            _batchProcessor.Dispose();
            base.Dispose();
        }
    }
}
