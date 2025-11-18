using EmailService.Core.Interfaces;
using EmailService.Infrastructure.Smtp.Options;
using EventBus.Messages.Abstraction.Events;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Infrastructure.Smtp.Smtp
{
    internal sealed class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly Lazy<IMailTransport> client;

        public SmtpEmailSender(IOptions<SmtpOptions> opts, ILogger<SmtpEmailSender> logger)
        {
            _options = opts.Value;
            _logger = logger;
            client = new Lazy<IMailTransport>(() =>
            {
                var smtpClient = new SmtpClient();
                smtpClient.Connect(this._options.Host, this._options.Port,
                    SecureSocketOptions.StartTlsWhenAvailable);
                smtpClient.Authenticate(this._options.Username, this._options.Password);
                smtpClient.NoOp();
                smtpClient.Timeout = _options.SendTimeoutMs;

                return smtpClient;
            });
        }

        public async Task SendAsync(EmailMessageEvent message, CancellationToken cancellationToken = default)
        {
            try
            {
                var mime = BuildMime(message);

                _logger.LogDebug("Sending email MessageId={MessageId} to={To}", message.MessageId, message.Recipient);

                var cl = client.Value;

                await client.Value.SendAsync(mime, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SMTP send failed for MessageId={MessageId}", message.MessageId);
                throw;
            }
        }

        private MimeMessage BuildMime(EmailMessageEvent msg)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_options.FromDisplayName, _options.FromAddress));
            mime.To.Add(MailboxAddress.Parse(msg.Recipient));
            mime.Subject = msg.Subject ?? string.Empty;
            var body = new TextPart(msg.IsHtml ? "html" : "plain") { Text = msg.Body ?? string.Empty };
            mime.Body = body;
            mime.MessageId = $"<{msg.MessageId}@{_options.Host}>";

            return mime;
        }

        public void Dispose()
        {
            try
            {
                client.Value.Disconnect(true);
            }
            catch { }

            client.Value.Dispose();
        }
    }
}