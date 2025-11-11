using EmailService.Infrastructure.RabbitMQ.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EmailService.Infrastructure.RabbitMQ
{
    public static class InfrastructureRabbitMQServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
            {
                Port = 5672,
                HostName = "localhost",
                UserName = "guest",
                Password = "password",
                Uri = new Uri("amqp://guest:guest@localhost:5672/")
            });

            services.AddHostedService<RabbitMqListener>();

            services
                .AddHealthChecks()
                .AddRabbitMQ();
            return services;
        }
    }
}
