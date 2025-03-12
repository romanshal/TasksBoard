using Authentication.API.Configurations;
using Authentication.Domain.Entities;
using Authentication.Infrastructure;
using Authentication.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddIdentityServer(options =>
{
    options.UserInteraction.LoginUrl = "/Authentication/Login";
    options.UserInteraction.LogoutUrl = "/Authentication/Logout";

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

var app = builder.Build();

//app.MigrateDatabase<AuthenticationDbContext>((context, services) =>
//{
//    var logger = services.GetService<ILogger<DataSeedMaker>>();
//    DataSeedMaker
//        .SeedAsync(context, logger!)
//        .Wait();
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseIdentityServer();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}")
    .WithStaticAssets();

app.Run();
