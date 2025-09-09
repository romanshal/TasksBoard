using Common.Cache.Configurations;
using Common.Cache.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace Common.Cache.Extensions
{
    public static class StackExchengeCacheExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection("Cache");
            var cacheSettings = section.Get<CacheConfiguration>()!;
            services.Configure<CacheConfiguration>(section);

            services.AddStackExchangeRedisCache(options =>
            {
                var connection = $"{cacheSettings.Redis.Host}:{cacheSettings.Redis.Port},password={cacheSettings.Redis.Password}";
                options.Configuration = connection;
            });

            var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { $"{cacheSettings.Redis.Host}:{cacheSettings.Redis.Port}" },
                AbortOnConnectFail = false,
                Ssl = false,
                Password = cacheSettings.Redis.Password,
                TieBreaker = "",
                AllowAdmin = true,
                CommandMap = CommandMap.Create(
                [
                    "INFO", "CONFIG", "CLUSTER", "PING", "ECHO", "CLIENT"
                ], available: false)
            });

            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

            services
                .AddHealthChecks()
                .AddRedis($"{cacheSettings.Redis.Host}:{cacheSettings.Redis.Port}");

            return services;
        }
    }
}
