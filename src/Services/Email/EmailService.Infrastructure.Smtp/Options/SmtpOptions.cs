namespace EmailService.Infrastructure.Smtp.Options
{
    internal sealed class SmtpOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 25;
        public bool UseSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int MaxPoolSize { get; set; } = 5;
        public int SendTimeoutMs { get; set; } = 30000;
    }
}
