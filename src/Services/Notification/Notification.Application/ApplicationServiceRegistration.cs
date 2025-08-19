using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.BackgroundServices;
using Notification.Application.Handlers;
using Notification.Application.Providers;
using System.Reflection;

namespace Notification.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddScoped<UserProfileHandler>();

            services.AddHostedService<NotificationBackgroundService>();

            return services;
        }
    }
}
