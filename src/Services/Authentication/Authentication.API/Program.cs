using Authentication.API.Configurations;
using Authentication.Domain.Entities;
using Authentication.Infrastructure;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.Extensions;
using Common.Blocks.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var conntectionString = builder.Configuration.GetConnectionString("AuthenticationDbConnection") ?? throw new InvalidOperationException("Connection string 'AuthenticationDbConnection' not found");
builder.Services.AddInfrastructureServices(conntectionString);

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

builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(config =>
    {
        config.Cookie.Name = "IdentityServer.Cookies";
    });
//.AddGoogle(config =>
//{
//    config.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientId' not found.");
//    config.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientSecret' not found.");
//});

//var migrationAssembly = typeof(Program).Assembly.GetName().Name;
builder.Services.AddIdentityServer(options =>
{
    options.UserInteraction.LoginUrl = "/Authentication/Login";
    options.UserInteraction.LogoutUrl = "/Authentication/Logout";

    //options.KeyManagement.Enabled = false;

    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
    .AddAspNetIdentity<ApplicationUser>()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiResources(IdentityServerConfiguration.ApiResources)
    .AddInMemoryIdentityResources(IdentityServerConfiguration.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfiguration.ApiScopes)
    .AddInMemoryClients(IdentityServerConfiguration.Clients);
//.AddConfigurationStore(options =>
//{
//    options.ConfigureDbContext = b => b.UseNpgsql(conntectionString,
//        sql => sql.MigrationsAssembly(migrationAssembly));
//})
//.AddOperationalStore(options =>
//{
//    options.ConfigureDbContext = b => b.UseNpgsql(conntectionString,
//        sql => sql.MigrationsAssembly(migrationAssembly));
//})


var app = builder.Build();

app.MigrateDatabase<AuthenticationDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DataSeedMaker>>();
    DataSeedMaker
        .SeedAsync(context, logger!)
        .Wait();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseExeptionWrappingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseIdentityServer();

app.MapStaticAssets();

app.MapControllers();

app.Run();
