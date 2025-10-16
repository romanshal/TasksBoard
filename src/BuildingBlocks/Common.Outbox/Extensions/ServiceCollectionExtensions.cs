using Common.Outbox.Factories;
using Common.Outbox.Interfaces.Factories;
using Common.Outbox.Services;
using Common.Outbox.ValueObjects;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

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

    public class OutboxIdTypeHandler : SqlMapper.TypeHandler<OutboxId>
    {
        public override void SetValue(IDbDataParameter parameter, OutboxId value)
        => parameter.Value = value.Value;

        public override OutboxId Parse(object value)
            => OutboxId.Of((Guid)value);
    }
}
