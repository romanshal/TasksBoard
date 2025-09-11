using Common.Blocks.Middlewares;
using EventBus.Messages.Extensions;
using Notification.Consumer.Builders;
using Notification.Consumer.Services;
using System.Reflection;
using static Common.gRPC.Protos.NotificationGrpc;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();
