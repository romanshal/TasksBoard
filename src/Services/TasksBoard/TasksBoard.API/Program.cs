using Common.Blocks.Configurations;
using Common.Blocks.Extensions;
using Common.Blocks.Middlewares;
using Common.Monitoring.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TasksBoard.Application;
using TasksBoard.Infrastructure;
using TasksBoard.Infrastructure.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLogging(builder.Configuration, "TasksBoards.API", builder.Environment.EnvironmentName)
    .AddApiMetrics(builder.Configuration, "TasksBoards.API", "0.1.0", builder.Environment.EnvironmentName);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200")
        //.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGetWithAuth("TasksBoards.API");

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

var jwt = builder.Configuration.GetRequiredSection("Authentication:Jwt").Get<JwtCofiguration>() ?? throw new InvalidOperationException("Configuration section 'Jwt' not found.");

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("role", "admin");
    });

var app = builder.Build();

app.MigrateDatabase<TasksBoardDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DataSeedMaker>>();
    DataSeedMaker
        .SeedAsync(context, logger!)
        .Wait();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TasksBoard.API v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseExeptionWrappingMiddleware();

app.MapControllers().RequireAuthorization();
app.MapPrometheusScrapingEndpoint();

app.UseHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program { }
