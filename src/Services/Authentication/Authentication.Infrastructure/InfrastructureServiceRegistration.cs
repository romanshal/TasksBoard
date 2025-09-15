using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Factories;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Authentication.Infrastructure.Data.Contexts;
using Authentication.Infrastructure.Factories;
using Authentication.Infrastructure.Repositories;
using Authentication.Infrastructure.Security;
using Authentication.Infrastructure.UnitOfWorks;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Common.Cache.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AuthenticationDbConnection") ?? throw new InvalidOperationException("Connection string 'AuthenticationDbConnection' not found");

            services.AddDbContext<AuthenticationDbContext>(options =>
                options.UseNpgsql(connectionString));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequiredLength = 8;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;

                config.User.RequireUniqueEmail = true;

                config.Lockout.AllowedForNewUsers = true;
                config.Lockout.MaxFailedAccessAttempts = 5;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders();

            services.AddRedis(configuration);

            services
                .AddTransient(typeof(IRepository<,>), typeof(Repository<,>))
                .AddTransient<IApplicationUserSessionRepository, ApplicationUserSessionRepository>()
                .AddTransient<IApplicationUserImageRepository, ApplicationUserImageRepository>();

            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUnitOfWorkBase>(sp => sp.GetRequiredService<IUnitOfWork>());

            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<ITokenFactory, TokenFactory>();
            services.AddScoped<IUserClaimsService, UserClaimsService>();

            services.AddSingleton<IDeviceFactory, DeviceFactory>();

            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
