using Common.Blocks.Behaviours;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TasksBoard.Application.Handlers;

namespace TasksBoard.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IOutboxService, OutboxService>();

            services.AddHostedService<OutboxPublisherService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<UserProfileHandler>();

            return services;
        }
    }
}
