﻿using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Infrastructure.Data.Contexts;
using Chat.Infrastructure.UnitOfWorks;
using Common.Blocks.Extensions;
using Common.Blocks.Interfaces.Caches;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
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
                options.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
