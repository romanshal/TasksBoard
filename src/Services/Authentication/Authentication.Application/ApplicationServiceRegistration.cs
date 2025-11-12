using Authentication.Application.Handlers;
using Common.Blocks.Behaviours;
using Common.Cache.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authentication.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
                conf.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                conf.AddOpenBehavior(typeof(CacheBehaviour<,>));
                conf.AddOpenBehavior(typeof(UpdateCacheBehaviour<,>));
            });

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<SignInHandler>();

            return services;
        }
    }
}
