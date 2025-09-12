using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Common.Cache.Extensions;
using Common.Cache.Interfaces;
using Common.Cache.Repositories;
using Common.gRPC.Interfaces.Caches;
using Common.gRPC.Interfaces.Services;
using Common.gRPC.Repositories;
using Common.gRPC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.UnitOfWorks;
using static Common.gRPC.Protos.UserProfiles;

namespace Notification.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserProfilesClient>(option =>
            {
                option.Address = new Uri(configuration["gRPC:Address"]!);
            });

            var connectionString = configuration.GetConnectionString("NotificationDbConnection") ?? throw new InvalidOperationException("Connection string 'NotificationDbConnection' not found");
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddRedis(configuration);

            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<ICacheRepository, RedisCacheRepository>();
            services.AddSingleton<IUserProfileCacheRepository, UserProfileCacheRepository>();

            services.AddScoped<IUserProfileService, UserProfileService>();

            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
