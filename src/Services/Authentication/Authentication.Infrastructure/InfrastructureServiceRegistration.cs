using Authentication.Application.Interfaces.Providers;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Authentication.Infrastructure.Data.Contexts;
using Authentication.Infrastructure.Repositories;
using Authentication.Infrastructure.Security;
using Authentication.Infrastructure.UnitOfWorks;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AuthenticationDbContext>(options =>
                options.UseNpgsql(connectionString));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IApplicationUserSessionRepository, ApplicationUserSessionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWorkBase>(sp => sp.GetRequiredService<IUnitOfWork>());

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            return services;
        }
    }
}
