using Common.Outbox.Factories;
using Common.Outbox.Interfaces.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Outbox.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services)
        {
            services.AddSingleton<IOutboxEventFactory, OutboxEventFactory>();

            return services;
        }
    }
}
