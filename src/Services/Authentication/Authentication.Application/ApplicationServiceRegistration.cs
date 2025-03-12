using Authentication.Application.Configurations;
using Authentication.Application.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authentication.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtCofiguration>(configuration.GetRequiredSection("Authentication:Jwt"));

            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<TokenProvider>();

            return services;
        }
    }
}
