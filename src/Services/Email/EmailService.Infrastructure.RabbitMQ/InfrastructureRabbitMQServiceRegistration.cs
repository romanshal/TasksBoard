using EmailService.Infrastructure.RabbitMQ.Listeners;
using EmailService.Infrastructure.RabbitMQ.Options;
using EmailService.Infrastructure.RabbitMQ.Proccessors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EmailService.Infrastructure.RabbitMQ
{
    public static class InfrastructureRabbitMQServiceRegistration
    {
        public static IServiceCollection AddInfrastructureRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSection = configuration.GetRequiredSection("RabbitMQ");
            var rabbtiMqOptions = rabbitMqSection.Get<RabbitMqOptions>()!;
            services.Configure<RabbitMqOptions>(rabbitMqSection);

            var batchSection = configuration.GetRequiredSection("Batch");
            services.Configure<BatchOptions>(batchSection);

            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
            {
                UserName = rabbtiMqOptions.Username,
                Password = rabbtiMqOptions.Password,
                Uri = new Uri(rabbtiMqOptions.Uri)
            });

            services.AddSingleton<RabbitMqConnectionManager>();
            services.AddSingleton<BatchProcessor>();

            services.AddHostedService<RabbitMqListener>();

            services
                .AddHealthChecks()
                .AddRabbitMQ();

            return services;
        }
    }
}
