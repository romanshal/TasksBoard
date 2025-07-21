using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
namespace Common.Blocks.Extensions
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddApiLogging(
            this IServiceCollection services, 
            IConfiguration configuration,
            IWebHostEnvironment environment,
            string applicationName)
        {
            services.AddLogging(cfg => cfg.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .WriteTo.Console()
                .WriteTo.Logger(cfg => cfg
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                    .WriteTo.GrafanaLoki(
                        configuration.GetConnectionString("Logs")!,
                        propertiesAsLabels: ["Application", "Environment"]
                    ))
                .CreateLogger()));

            return services;
        }
    }
}
