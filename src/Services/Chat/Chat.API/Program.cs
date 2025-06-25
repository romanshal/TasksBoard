using Chat.API.Hubs;
using Chat.Application;
using Chat.Infrastructure;
using Chat.Infrastructure.Data.Contexts;
using Common.Blocks.Configurations;
using Common.Blocks.Extensions;
using Common.Blocks.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(config =>
{
    config.AddConsole();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200")
        //.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddSignalR();

builder.Services.AddControllers();

builder.Services.AddSwaggerGetWithAuth("Chat API");

var connectionString = builder.Configuration.GetConnectionString("ChatDbConnection") ?? throw new InvalidOperationException("Connection string 'ChatDbConnection' not found");
builder.Services
    .AddInfrastructureServices(connectionString)
    .AddApplicationServices();

var jwt = builder.Configuration.GetRequiredSection("Authentication:Jwt").Get<JwtCofiguration>() ?? throw new InvalidOperationException("Configuration section 'Jwt' not found.");

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.MigrateDatabase<ChatDbContext>((context, services) =>
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat.API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseExeptionWrappingMiddleware();

app.MapHub<BoardChatHub>("/chat");

app.MapControllers()
    .RequireAuthorization();

app.Run();
