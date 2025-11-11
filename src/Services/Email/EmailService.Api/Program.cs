using EmailService.Infrastructure.Postgres;
using EmailService.Infrastructure.Postgres.Data.Contexts;
using Common.Blocks.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructurePostgresServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MigrateDatabase<EmailDbContext>();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
