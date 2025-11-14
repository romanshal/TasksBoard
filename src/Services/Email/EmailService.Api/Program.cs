using EmailService.Infrastructure.Postgres;
using EmailService.Infrastructure.RabbitMQ;
using EmailService.Infrastructure.Postgres.Data.Contexts;
using Common.Blocks.Extensions;
using EmailService.Infrastructure.Smtp;
using Common.Monitoring.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApiLogging(builder.Configuration, "EmailService.API", builder.Environment.EnvironmentName)
    .AddApiMetrics(builder.Configuration, "EmailService.API", "0.1.0", builder.Environment.EnvironmentName);

builder.Services
    .AddInfrastructurePostgresServices(builder.Configuration)
    .AddInfrastructureRabbitMQServices(builder.Configuration)
    .AddInfrastructureSmtpServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MigrateDatabase<EmailDbContext>();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
