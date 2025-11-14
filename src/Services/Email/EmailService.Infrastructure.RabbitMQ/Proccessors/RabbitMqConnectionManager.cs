using EmailService.Infrastructure.RabbitMQ.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EmailService.Infrastructure.RabbitMQ.Proccessors
{
    internal class RabbitMqConnectionManager : IDisposable
    {
        private readonly RabbitMqOptions _opts;
        private readonly IConnectionFactory _connectionFactory;

        public readonly string QueueName = string.Empty;
        public IConnection? Connection;
        public IChannel? Channel;

        public RabbitMqConnectionManager(IOptions<RabbitMqOptions> opts, IConnectionFactory connectionFactory)
        {
            _opts = opts.Value;
            _connectionFactory = connectionFactory;

            QueueName = _opts.QueueName;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            Connection = await _connectionFactory.CreateConnectionAsync("EmailService.API", cancellationToken);
            Channel = await Connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await Channel.BasicQosAsync(0, _opts.PrefetchCount, false, cancellationToken);
            await Channel.ExchangeDeclareAsync(QueueName, "fanout", true, false, cancellationToken: cancellationToken);
            await Channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await Channel.QueueBindAsync(QueueName, QueueName, "", cancellationToken: cancellationToken);
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (Channel != null)
                await Channel.CloseAsync(cancellationToken);
            if (Connection != null)
                await Connection.CloseAsync(cancellationToken);
        }

        public void Dispose()
        {
            Channel?.Dispose();
            Connection?.Dispose();
        }
    }
}
