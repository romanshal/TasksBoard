using Common.Blocks.Extensions;
using Common.Monitoring.Extensions;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using OcelotApiGateway.Configurations;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLogging(builder.Configuration, "OcelotGateway.API", builder.Environment.EnvironmentName)
    .AddApiMetrics(builder.Configuration, "OcelotGateway.API", "0.1.0", builder.Environment.EnvironmentName);

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

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddCookie();

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

app.MapPrometheusScrapingEndpoint();

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

