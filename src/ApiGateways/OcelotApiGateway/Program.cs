using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using OcelotApiGateway.Configurations;
using Polly;
using Common.Blocks.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps("/app/localhost-dev.pfx", "P@ssw0rd!");
    });
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("/app/localhost-dev.pfx", "P@ssw0rd!");
    });
});

var retryPolicy = builder.Configuration
    .GetSection("RetryPolicy")
    .Get<RetryPolicyConfiguration>() ?? throw new InvalidOperationException("Configurations 'RetryPolicy' not found.");

// Add services to the container.
builder.Configuration.AddJsonFile($"ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services
    .AddOcelot(builder.Configuration)
    .AddPolly()
    .AddCacheManager(cache =>
    {
        cache.WithDictionaryHandle();
    });

builder.Services.AddHttpClient("OcelotApiGatewayHttpClient")
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return Policy.Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(retryPolicy!.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(retryPolicy!.RetryAttemptTimeout, retryAttempt)));
}

// Определить политику автоматического выключателя для HTTP-запросов 
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return Policy.Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .CircuitBreakerAsync(retryPolicy!.HandleBeforeBreaking, TimeSpan.FromSeconds(retryPolicy.DurationOfBreak));
}

