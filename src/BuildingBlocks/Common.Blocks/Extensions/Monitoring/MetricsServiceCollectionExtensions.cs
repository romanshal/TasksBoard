using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Blocks.Extensions.Monitoring
{
    public static class MetricsServiceCollectionExtensions
    {
        public static IServiceCollection AddApiMetrics(
            this IServiceCollection services, 
            IConfiguration configuration, 
            string serviceName)
        {
            var jaegerUrl = configuration
                .GetRequiredSection("Jaeger")
                .GetRequiredSection("Url")
                .Get<string>()!;

            services
                .AddOpenTelemetry()
                .WithMetrics(builder => builder
                    .AddAspNetCoreInstrumentation()
                    ////.AddConsoleExporter()
                    .AddPrometheusExporter()
                    .AddView("https.serve.request.duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                    }))
                .WithTracing(builder => builder
                    .ConfigureResource(r => r.AddService(serviceName))
                    //.AddSource(serviceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter += context =>
                            !context.Request.Path.Value!.Contains("metrics", StringComparison.InvariantCultureIgnoreCase) &&
                            !context.Request.Path.Value!.Contains("swagger", StringComparison.InvariantCultureIgnoreCase);
                        options.EnrichWithHttpResponse = (activity, response) =>
                            activity.AddTag("error", response.StatusCode >= 400);
                    })
                    .AddEntityFrameworkCoreInstrumentation(optons => optons.SetDbStatementForText = true)
                    .AddConsoleExporter()
                    .AddOtlpExporter(cfg => cfg.Endpoint = new Uri(jaegerUrl)));

            return services;
        }
    }
}
