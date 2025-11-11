namespace EmailService.Infrastructure.RabbitMQ.Options
{
    internal sealed class RabbitMqOptions
    {
        public string Url { get; set; } = "amqp://guest:guest@localhost:5672";
        public string QueueName { get; set; } = "email.send";
        public ushort PrefetchCount { get; set; } = 20;
    }
}
