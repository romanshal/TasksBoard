using EmailService.Infrastructure.Postgres;
using EmailService.Infrastructure.RabbitMQ;
using EmailService.Infrastructure.Postgres.Data.Contexts;
using Common.Blocks.Extensions;
using EmailService.Infrastructure.Smtp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
