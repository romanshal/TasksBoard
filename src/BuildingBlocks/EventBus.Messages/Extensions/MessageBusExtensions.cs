using EventBus.Messages.Abstraction.Interfaces;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventBus.Messages.Extensions
{
    public static class MessageBusExtensions
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });
                    configurator.ConfigureEndpoints(context);
                });

                services.Configure<MassTransitHostOptions>(conf =>
                {
                    conf.WaitUntilStarted = true;

                    conf.StartTimeout = TimeSpan.FromSeconds(int.Parse(configuration["MessageBroker:StartTimeout"]!));
                    conf.StopTimeout = TimeSpan.FromMinutes(int.Parse(configuration["MessageBroker:StopTimeout"]!));
                });
            });

            return services;
        }
    }
}
