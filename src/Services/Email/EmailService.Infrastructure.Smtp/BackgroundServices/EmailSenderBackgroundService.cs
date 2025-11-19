using EmailService.Core.Constants;
using EmailService.Core.Interfaces;
using EmailService.Core.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailService.Infrastructure.Smtp.BackgroundServices
{
    internal class EmailSenderBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EmailSenderBackgroundService> logger) : BackgroundService
    {
        private readonly ILogger<EmailSenderBackgroundService> _logger = logger;

        private readonly int _pollingDelayMs = 10000;
        private readonly int _batchSize = 100;
        private readonly int _nextAttemptSeconds = 60;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            EmailSenderLoggerMessages.StartLogging(_logger);

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var outboxRespository = scope.ServiceProvider.GetRequiredService<IOutboxRespository>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await outboxRespository.FetchAndClaimBatchAsync(_batchSize, stoppingToken);

                    if (messages.Count == 0)
                    {
                        await Task.Delay(_pollingDelayMs, stoppingToken);
                        continue;
                    }

                    foreach (var message in messages)
                    {
                        try
                        {
                            await emailSender.SendAsync(message, stoppingToken);
                            await outboxRespository.MarkSentAsync(message.MessageId, stoppingToken);
                            EmailSenderLoggerMessages.LogSuccess(_logger, message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            EmailSenderLoggerMessages.LogSendError(_logger, ex, message.MessageId);
                            await outboxRespository.MarkFailedAsync(message.MessageId, ex.Message, nextAttemptSeconds: _nextAttemptSeconds, stoppingToken);
                            continue;
                        }
                    }
                }
                catch(OperationCanceledException ex)
                {
                    EmailSenderLoggerMessages.LogOperationCancelled(_logger);
                    break;
                }
                catch (Exception ex)
                {
                    EmailSenderLoggerMessages.LogError(_logger, ex);
                    await Task.Delay(_pollingDelayMs, stoppingToken);
                }
            }
        }
    }
}
