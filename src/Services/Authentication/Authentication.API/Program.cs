using Authentication.Application;
using Authentication.Domain.Entities;
using Authentication.Infrastructure;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.Extensions;
using Common.Blocks.Extensions.Monitoring;
using Common.Blocks.Middlewares;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiLogging(builder.Configuration, builder.Environment, "Authentication.API");

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddSwaggerGetWithAuth("Authentication API");

var conntectionString = builder.Configuration.GetConnectionString("AuthenticationDbConnection") ?? throw new InvalidOperationException("Connection string 'AuthenticationDbConnection' not found");

builder.Services
    .AddInfrastructureServices(conntectionString)
    .AddApplicationServices(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AuthenticationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration)
    .AddGoogle(config =>
    {
        config.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientId' not found.");
        config.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientSecret' not found.");
    });

var app = builder.Build();

app.MigrateDatabase<AuthenticationDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DataSeedMaker>>();
    DataSeedMaker
        .SeedAsync(context, logger!)
        .Wait();
});

app.UseCors("AllowSpecificOrigin");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication.API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseExeptionWrappingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
