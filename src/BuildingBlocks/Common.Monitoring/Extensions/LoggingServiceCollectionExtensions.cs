using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSearch.Net;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.OpenSearch;
namespace Common.Monitoring.Extensions
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddApiLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            string applicationName,
            string environmentName)
        {
            //var grafanaSection = configuration.GetRequiredSection("Grafana");
            //var grafanaUrl = grafanaSection.GetRequiredSection("Url").Get<string>()!;

            var osSection = configuration.GetRequiredSection("OpenSearch");
            var osUrls = osSection
                .GetSection("Urls")
                .Get<string>()!
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(u => new Uri(u.Trim()))
                .ToArray();
            var connectionPool = new StaticConnectionPool(osUrls);
            var indexFormat = osSection["IndexFormat"]!;
            var login = osSection["Login"]!;
            var password = osSection["Password"]!;

            services
                .AddLogging(cfg => cfg
                .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
                .AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Environment", environmentName)
                //.WriteTo.Logger(cfg => cfg
                //    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                //    .WriteTo.GrafanaLoki(
                //        grafanaUrl,
                //        propertiesAsLabels: ["Application", "Environment"]
                //    ))
                .WriteTo.Logger(cfg => cfg
                    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                    .WriteTo.OpenSearch(new OpenSearchSinkOptions(connectionPool)
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.OSv1,
                        MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                        TypeName = null,
                        InlineFields = false,
                        ModifyConnectionSettings = x =>
                            x.BasicAuthentication(login, password)
                                .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true),
                        IndexFormat = indexFormat,
                        Period = TimeSpan.FromSeconds(5),
                        BatchPostingLimit = 50
                    }))
                .CreateLogger()));

            return services;
        }
    }
}
