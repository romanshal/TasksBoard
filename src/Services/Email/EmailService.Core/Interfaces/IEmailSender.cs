using EmailService.Core.Models;

namespace EmailService.Core.Interfaces
{
    /// <summary>
    /// Send emails to users.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Send a single email. Throw on permanent failures.
        /// Transient failures may also throw and will be handled by outbox retry logic.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}
