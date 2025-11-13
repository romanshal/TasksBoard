namespace EmailService.Infrastructure.RabbitMQ.Options
{
    public class BatchOptions
    {
        public int BatchSize { get; set; } = 50;
        public int FlushIntervalMs { get; set; } = 5000;
    }
}
