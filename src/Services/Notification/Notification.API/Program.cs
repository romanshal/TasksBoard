using Common.Blocks.Configurations;
using Common.Blocks.Extensions;
using Common.Blocks.Middlewares;
using Notification.API.Hubs;
using Notification.API.HubServices;
using Notification.Application;
using Notification.Application.Interfaces.HubServices;
using Notification.Infrastructure;
using Notification.Infrastructure.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiLogging(builder.Configuration, builder.Environment, "Notification.API");

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddSignalR();

builder.Services.AddTransient<IHubNotificationService, HubNotificationService>();

builder.Services.AddSwaggerGetWithAuth("Notification API");

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

var jwt = builder.Configuration.GetRequiredSection("Authentication:Jwt").Get<JwtCofiguration>() ?? throw new InvalidOperationException("Configuration section 'Jwt' not found.");

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.MigrateDatabase<NotificationDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DataSeedMaker>>();
    DataSeedMaker
        .SeedAsync(context, logger!)
        .Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification.API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseExeptionWrappingMiddleware();

app.MapHub<NotificationHub>("/notification");

app.MapControllers()
    .RequireAuthorization();

app.Run();
