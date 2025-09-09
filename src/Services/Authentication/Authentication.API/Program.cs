using Authentication.API.Controllers;
using Authentication.Application;
using Authentication.Domain.Entities;
using Authentication.Infrastructure;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.Extensions;
using Common.Blocks.Extensions.Monitoring;
using Common.Blocks.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApiLogging(builder.Configuration, builder.Environment, "Authentication.API")
    .AddApiMetrics(builder.Configuration, "Authentication.API");

builder.Services.AddControllers();

builder.Services.AddGrpc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins(
            "http://localhost:4200", 
            "https://localhost:9005", 
            "https://ocelotapigateway:443", 
            "http://ocelotapigateway:80", 
            "http://ocelotapigateway", 
            "https://ocelotapigateway",
            "http://localhost",
            "https://localhost")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddSwaggerGetWithAuth("Authentication API");

var conntectionString = builder.Configuration.GetConnectionString("AuthenticationDbConnection") ?? throw new InvalidOperationException("Connection string 'AuthenticationDbConnection' not found");

builder.Services
    .AddInfrastructureServices(conntectionString)
    .AddApplicationServices(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
{
    config.Password.RequiredLength = 8;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireLowercase = false;

    config.User.RequireUniqueEmail = true;

    config.Lockout.AllowedForNewUsers = true;
    config.Lockout.MaxFailedAccessAttempts = 5;
    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<AuthenticationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration)
    .AddCookie()
    .AddGoogle("Google", config =>
    {
        config.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientId' not found.");
        config.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientSecret' not found.");
        //config.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"] ?? throw new InvalidOperationException("Configuration setting 'Google:CallbackPath' not found.");
        config.SignInScheme = IdentityConstants.ExternalScheme;
        config.Scope.Add("email");
        config.Scope.Add("profile");
        config.ClaimActions.MapJsonKey("email_verified", "email_verified");
    });
    //.AddFacebook("Facebook", fb =>
    //{
    //    fb.SignInScheme = IdentityConstants.ExternalScheme;
    //    fb.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
    //    fb.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    //    fb.Fields.Add("email");
    //    fb.Fields.Add("name");
    //});

builder.Services.ConfigureExternalCookie(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

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
else
{
    app.UseHttpsRedirection();
}

app.UseExeptionWrappingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.MapGrpcService<UserProfilesGrpñController>();
//app.MapGrpcReflectionService();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
