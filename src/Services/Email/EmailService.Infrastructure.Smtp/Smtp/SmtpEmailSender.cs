using EmailService.Core.Interfaces;
using EmailService.Core.Models;
using EmailService.Infrastructure.Smtp.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Collections.Concurrent;

namespace EmailService.Infrastructure.Smtp.Smtp
{
    internal sealed class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpOptions _options;
        private readonly ConcurrentBag<SmtpClient> _clients = [];
        private readonly SemaphoreSlim _poolSemaphore;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpOptions> opts, ILogger<SmtpEmailSender> logger)
        {
            _options = opts.Value;
            _logger = logger;
            _poolSemaphore = new SemaphoreSlim(_options.MaxPoolSize, _options.MaxPoolSize);
        }

        public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            await _poolSemaphore.WaitAsync(cancellationToken);

            SmtpClient? client = null;

            try
            {
                client = await RentClientAsync(cancellationToken);

                var mime = BuildMime(message);

                client.Timeout = _options.SendTimeoutMs;

                _logger.LogDebug("Sending email MessageId={MessageId} To={To}", message.MessageId, message.To);

                await client.SendAsync(mime, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SMTP send failed for MessageId={MessageId}", message.MessageId);
                throw;
            }
            finally
            {
                if (client is not null) ReturnClient(client);
                _poolSemaphore.Release();
            }
        }

        private MimeMessage BuildMime(EmailMessage msg)
        {
            var mime = new MimeMessage();
            mime.From.Add(MailboxAddress.Parse(msg.From));
            mime.To.Add(MailboxAddress.Parse(msg.To));
            mime.Subject = msg.Subject ?? string.Empty;
            var body = new TextPart(msg.IsHtml ? "html" : "plain") { Text = msg.Body ?? string.Empty };
            mime.Body = body;
            mime.MessageId = $"<{msg.MessageId}@{_options.Host}>";

            return mime;
        }

        private async Task<SmtpClient> RentClientAsync(CancellationToken cancellationToken = default)
        {
            if (_clients.TryTake(out var client))
            {
                if (client.IsConnected) return client;

                try 
                { 
                    await client.DisconnectAsync(true, cancellationToken); 
                } 
                catch { }

                client.Dispose();
            }

            var newClient = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true // production: validate certs properly
            };

            await newClient.ConnectAsync(
                _options.Host, 
                _options.Port, 
                _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, 
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await newClient.AuthenticateAsync(_options.Username, _options.Password ?? string.Empty, cancellationToken);
            }

            return newClient;
        }

        private void ReturnClient(SmtpClient client)
        {
            if (client.IsConnected)
            {
                _clients.Add(client);
                return;
            }

            try 
            {
                client.Dispose(); 
            } 
            catch { }
        }

        public void Dispose()
        {
            while (_clients.TryTake(out var client))
            {
                try 
                {
                    client.Disconnect(true); 
                } 
                catch { }

                client.Dispose();
            }

            _poolSemaphore.Dispose();
        }
    }
}
