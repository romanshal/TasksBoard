using EmailService.Core.Interfaces.Repositories;
using EmailService.Infrastructure.Postgres.Data.Contexts;
using EmailService.Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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

            services.AddSingleton(_ =>
            {
                return new NpgsqlDataSourceBuilder(connectionString).Build();
            });

            services.AddTransient<IInboxRepository, InboxRepository>();
            services.AddTransient<IOutboxRespository, OutboxRepository>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services
                .AddHealthChecks()
                .AddNpgSql(connectionString);

            return services;
        }
    }
}
