using Common.Blocks.Behaviours;
using Common.Cache.Behaviours;
using Common.Outbox.Interfaces.Services;
using Common.Outbox.Services;
using FluentValidation;
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
                conf.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                conf.AddOpenBehavior(typeof(CacheBehaviour<,>));
            });

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddTransient<IOutboxService, OutboxService>();

            //services.AddHostedService<OutboxPublisherService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IUserProfileHandler, UserProfileHandler>();

            return services;
        }
    }
}
