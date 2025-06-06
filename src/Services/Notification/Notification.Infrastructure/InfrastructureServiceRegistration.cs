using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.UnitOfWorks;

namespace Notification.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
