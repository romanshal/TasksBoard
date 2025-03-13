using Common.Blocks.Extensions;
using Common.Blocks.Middlewares;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using TasksBoard.API.Behaviours;
using TasksBoard.Application;
using TasksBoard.Infrastructure;
using TasksBoard.Infrastructure.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.AddConsole();
});

builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddSwaggerGetWithAuth("TasksBoards API");

var connectionString = builder.Configuration.GetConnectionString("TasksBoardDbConnection") ?? throw new InvalidOperationException("Connection string 'TasksBoardDbConnection' not found");
builder.Services
    .AddInfrastructureServices(connectionString)
    .AddApplicationServices();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.Authority = builder.Configuration["Authentication:Jwt:Authority"] ?? throw new InvalidOperationException("Configuration setting 'Authority' not found");
        options.Audience = builder.Configuration["Authentication:Jwt:Audience"] ?? throw new InvalidOperationException("Configuration setting 'Audience' not found");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            //NameClaimType = JwtClaimTypes.Name,
            //RoleClaimType = JwtClaimTypes.Role,
        };
    });

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

app.UseHttpsRedirection();

app.UseExeptionWrappingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder => builder.AllowAnyOrigin());

app.MapControllers();

app.Run();
