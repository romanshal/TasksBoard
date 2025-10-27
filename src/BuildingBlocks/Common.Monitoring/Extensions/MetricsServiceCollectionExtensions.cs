using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Common.Monitoring.Extensions
{
    public static class MetricsServiceCollectionExtensions
    {
        public static IServiceCollection AddApiMetrics(
            this IServiceCollection services,
            IConfiguration configuration,
            string serviceName,
            string serviceVersion,
            string environmentName)
        {
            var jaegerUrl = configuration
                .GetRequiredSection("Jaeger")
                .GetRequiredSection("Url")
                .Get<string>()!;

            services
                .AddOpenTelemetry()
                .WithMetrics(builder => builder
                    .AddAspNetCoreInstrumentation()
                    //.AddConsoleExporter()
                    .AddPrometheusExporter()
                    .AddView("https.serve.request.duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                    }))
                .WithTracing(builder => builder
                    .ConfigureResource(r =>
                    {
                        r.AddService(serviceName: serviceName, serviceVersion: serviceVersion);
                        r.AddAttributes(
                        [
                            new KeyValuePair<string, object>("deployment.environment", environmentName),
                            new KeyValuePair<string, object>("service.instance.id", Environment.MachineName)
                        ]);
                    })
                    .AddSource(serviceName)
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter += context =>
                            !context.Request.Path.Value!.Contains("metrics", StringComparison.InvariantCultureIgnoreCase) &&
                            !context.Request.Path.Value!.Contains("swagger", StringComparison.InvariantCultureIgnoreCase);
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request_content_length", request.ContentLength);
                            activity.SetTag("enduser.id", request.HttpContext.User.Identity?.Name);
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            if (response.StatusCode >= 200 && response.StatusCode < 300)
                            {
                                activity.SetStatus(ActivityStatusCode.Ok);
                            }
                            else if (response.StatusCode >= 400 && response.StatusCode < 500)
                            {
                                activity.AddTag("warning", response.StatusCode >= 400 && response.StatusCode < 500);
                            }
                            else if (response.StatusCode >= 500)
                            {
                                activity.SetStatus(ActivityStatusCode.Error);
                            }
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation(optons =>
                    {
                        optons.SetDbStatementForText = true;
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(jaegerUrl);
                    }));

            return services;
        }
    }
}
