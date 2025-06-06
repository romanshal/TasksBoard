using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using EventBus.Messages.Extensions;
using Common.Blocks.Services;
using Common.Blocks.Interfaces.Services;

namespace TasksBoard.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddTransient<IOutboxService, OutboxService>();

            services.AddHostedService<OutboxPublisherService>();

            return services;
        }
    }
}
