using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Infrastructure.Data.Contexts;
using Chat.Infrastructure.UnitOfWorks;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChatDbContext>(options =>
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
