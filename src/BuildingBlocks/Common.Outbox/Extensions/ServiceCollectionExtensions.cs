using Common.Outbox.Abstraction.Interfaces.Factories;
using Common.Outbox.Factories;
using Common.Outbox.Handlers;
using Common.Outbox.Services;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Common.Outbox.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IOutboxEventFactory, OutboxEventFactory>();

            services.AddSingleton(_ =>
            {
                return new NpgsqlDataSourceBuilder(connectionString).Build();
            });

            services.AddScoped<OutboxProcessor>();

            services.AddHostedService<OutboxPublisherBackgroundService>();

            SqlMapper.AddTypeHandler(new OutboxIdTypeHandler());

            return services;
        }
    }
}
