using Chat.Domain.Interfaces.Repositories;
using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Infrastructure.Data.Contexts;
using Chat.Infrastructure.Repositories;
using Chat.Infrastructure.UnitOfWorks;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Common.Cache.Extensions;
using Common.Cache.Interfaces.Repositories;
using Common.Cache.Repositories;
using Common.gRPC.Interfaces.Caches;
using Common.gRPC.Interfaces.Services;
using Common.gRPC.Repositories;
using Common.gRPC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Common.gRPC.Protos.UserProfiles;

namespace Chat.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserProfilesClient>(option =>
            {
                option.Address = new Uri(configuration["gRPC:Address"]!);
            });

            services.AddRedis(configuration);

            services.AddSingleton<ICacheRepository, RedisCacheRepository>();
            services.AddSingleton<IUserProfileCacheRepository, UserProfileCacheRepository>();

            services.AddScoped<IUserProfileService, UserProfileService>();

            var connectionString = configuration.GetConnectionString("ChatDbConnection") ?? throw new InvalidOperationException("Connection string 'ChatDbConnection' not found");
            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient<IBoardMessageRepository, BoardMessageRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
