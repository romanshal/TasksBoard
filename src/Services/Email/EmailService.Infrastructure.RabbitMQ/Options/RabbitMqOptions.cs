namespace EmailService.Infrastructure.RabbitMQ.Options
{
    internal sealed class RabbitMqOptions
    {   
        public string Uri { get; set; } = "amqp://guest:guest@localhost:5672";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string QueueName { get; set; } = "email.send";
        public ushort PrefetchCount { get; set; } = 20;
    }
}
