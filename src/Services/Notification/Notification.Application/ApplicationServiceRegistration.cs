using Common.Blocks.Services;
using EventBus.Messages.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.BackgroundServices;
using Notification.Application.Providers;
using System.Reflection;

namespace Notification.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddHostedService<NotificationBackgroundService>();

            return services;
        }
    }
}
