using Authentication.Application.Interfaces.Providers;
using Authentication.Application.Interfaces.Services;
using Authentication.Application.Providers;
using Authentication.Application.Services;
using Authentication.Domain.Interfaces.Repositories;
using Common.Blocks.Behaviours;
using Common.Blocks.Configurations;
using EventBus.Messages.Extensions;
using FluentValidation;
using MediatR;
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

            services.AddScoped<IUserClaimsService, UserClaimsService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            //services.AddHostedService<OutboxPublisherService>();

            return services;
        }
    }
}
