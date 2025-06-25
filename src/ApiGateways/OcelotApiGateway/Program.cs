using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration).AddCacheManager(cache =>
{
    cache.WithDictionaryHandle();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

await app.UseOcelot();

app.Run();
