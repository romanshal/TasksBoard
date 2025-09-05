﻿using Common.Blocks.Extensions;
using Common.Blocks.Interfaces.Caches;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Common.gRPC.Interfaces.Caches;
using Common.gRPC.Interfaces.Services;
using Common.gRPC.Repositories;
using Common.gRPC.Services;
using EventBus.Messages.Extensions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Infrastructure.Data.Contexts;
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
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true
                };
            }).ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
            }); ;

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
