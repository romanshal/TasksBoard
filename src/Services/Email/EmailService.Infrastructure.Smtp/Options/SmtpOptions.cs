namespace EmailService.Infrastructure.Smtp.Options
{
    /// <summary>
    /// SMTP configuration
    /// </summary>
    internal sealed class SmtpOptions
    {
        /// <summary>
        /// SMTP server host
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// SMTP user name
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// SMTP user password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Send emails from address
        /// </summary>
        public required string FromAddress { get; set; }

        /// <summary>
        /// Display name for emails sender
        /// </summary>
        public required string FromDisplayName { get; set; }

        /// <summary>
        /// SMTP timeout
        /// </summary>
        public int SendTimeoutMs { get; set; } = 30000;
    }
}
