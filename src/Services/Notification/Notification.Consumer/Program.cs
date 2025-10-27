using Common.Blocks.Middlewares;
using Common.Monitoring.Extensions;
using EventBus.Messages.Extensions;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using System.Reflection;
using static Common.gRPC.Protos.NotificationGrpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLogging(builder.Configuration, "Notification.Consumer", builder.Environment.EnvironmentName)
    .AddApiMetrics(builder.Configuration, "Notification.Consumer", "0.1.0", builder.Environment.EnvironmentName);

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddGrpcClient<NotificationGrpcClient>(option =>
{
    option.Address = new Uri(builder.Configuration["gRPC:Address"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

builder.Services.AddScoped<IGrpcService, GrpcService>();
builder.Services.AddTransient(typeof(IGrpcRequestBuilder<>), typeof(GrpcRequestBuilder<>));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseExeptionWrappingMiddleware();

app.MapPrometheusScrapingEndpoint();

app.Run();
