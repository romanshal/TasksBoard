using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Common.Blocks.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>()!;

                try
                {
                    logger.LogInformation("Migrating database associated with context {context}", typeof(TContext).Name);

                    var retry = Policy.Handle<NpgsqlException>()
                            .WaitAndRetry(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry: (exception, retryCount, context) =>
                                {
                                    logger.LogError("Retry {retry} of {policyKey} at {operationKey}, due to: {exception}.", retryCount, context.PolicyKey, context.OperationKey, exception);
                                });

                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database associated with context {context}", typeof(TContext).Name);
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {context}", typeof(TContext).Name);
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
