using Common.Blocks.Contexts;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Common.Outbox.Abstraction.Interfaces.Factories;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Factories;
using Common.Outbox.Handlers;
using Common.Outbox.Repositories;
using Common.Outbox.Services;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Common.Outbox.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutbox<TContext>(this IServiceCollection services, string connectionString) where TContext : CommonDbContext<TContext>
        {
            services.AddSingleton<IOutboxEventFactory, OutboxEventFactory>();

            services.AddSingleton(_ =>
            {
                return new NpgsqlDataSourceBuilder(connectionString).Build();
            });

            services.AddScoped<OutboxProcessor>();

            services.AddTransient<IOutboxEventRepository, OutboxEventRepository<TContext>>();

            services.AddHostedService<OutboxPublisherBackgroundService>();

            SqlMapper.AddTypeHandler(new OutboxIdTypeHandler());

            return services;
        }
    }
}
