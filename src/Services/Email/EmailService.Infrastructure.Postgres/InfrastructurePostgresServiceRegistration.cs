using EmailService.Infrastructure.Postgres.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService.Infrastructure.Postgres
{
    public static class InfrastructurePostgresServiceRegistration
    {
        public static IServiceCollection AddInfrastructurePostgresServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EmailDbConnection") ?? throw new InvalidOperationException("Connection string 'EmailDbConnection' not found");
            services.AddDbContext<EmailDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseSnakeCaseNamingConvention();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
