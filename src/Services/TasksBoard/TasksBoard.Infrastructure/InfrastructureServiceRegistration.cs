using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Common.Cache.Extensions;
using Common.gRPC.Interfaces.Caches;
using Common.gRPC.Interfaces.Services;
using Common.gRPC.Repositories;
using Common.gRPC.Services;
using EventBus.Messages.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Infrastructure.Data.Contexts;
using TasksBoard.Infrastructure.Repositories;
using TasksBoard.Infrastructure.UnitOfWorks;
using static Common.gRPC.Protos.UserProfiles;

namespace TasksBoard.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserProfilesClient>(option =>
            {
                option.Address = new Uri(configuration["gRPC:Address"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return handler;
            });

            var connectionString = configuration.GetConnectionString("TasksBoardDbConnection") ?? throw new InvalidOperationException("Connection string 'TasksBoardDbConnection' not found");
            services.AddDbContext<TasksBoardDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddRedis(configuration);

            services
                .AddSingleton<IUserProfileCacheRepository, UserProfileCacheRepository>();

            services.AddScoped<IUserProfileService, UserProfileService>();

            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUnitOfWorkBase>(sp => sp.GetRequiredService<IUnitOfWork>());

            services
                .AddTransient(typeof(IRepository<,>), typeof(Repository<,>))
                .AddTransient<IBoardRepository, BoardRepository>()
                .AddTransient<IBoardNoticeRepository, BoardNoticeRepository>()
                .AddTransient<IBoardMemberRepository, BoardMemberRepository>()
                .AddTransient<IBoardInviteRequestRepository, BoardInviteRequestRepository>()
                .AddTransient<IBoardAccessRequestRepository, BoardAccessRequestRepsitory>();


            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
