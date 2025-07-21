using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using EventBus.Messages.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.UnitOfWorks;
using System.Reflection;

namespace Notification.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NotificationDbConnection") ?? throw new InvalidOperationException("Connection string 'NotificationDbConnection' not found");
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
