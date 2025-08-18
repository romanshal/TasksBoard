using Common.Blocks.Extensions;
using Common.Blocks.Interfaces.Caches;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using EventBus.Messages.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Infrastructure.Data.Contexts;
using TasksBoard.Infrastructure.Services;
using TasksBoard.Infrastructure.UnitOfWorks;
using static Common.Blocks.Protos.UserProfiles;

namespace TasksBoard.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserProfilesClient>(option =>
            {
                option.Address = new Uri(configuration["gRPC:Address"]!);
            });

            var connectionString = configuration.GetConnectionString("TasksBoardDbConnection") ?? throw new InvalidOperationException("Connection string 'TasksBoardDbConnection' not found");
            services.AddDbContext<TasksBoardDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWorkBase>(sp => sp.GetRequiredService<IUnitOfWork>());

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddRedis(configuration);

            services.AddSingleton<ICacheRepository, RedisCacheRepository>();
            services.AddSingleton<IUserProfileCacheRepository, UserProfileCacheRepository>();

            services.AddScoped<IUserProfileService, UserProfileService>();

            services.AddHealthChecks()
                .AddNpgSql(connectionString)
                .AddRabbitMQ();

            return services;
        }
    }
}
