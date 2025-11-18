using EmailService.Core.Interfaces;
using EmailService.Infrastructure.Smtp.BackgroundServices;
using EmailService.Infrastructure.Smtp.Options;
using EmailService.Infrastructure.Smtp.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService.Infrastructure.Smtp
{
    public static class InfrastructureSmtpServiceRegistration
    {
        public static IServiceCollection AddInfrastructureSmtpServices(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpSection = configuration.GetRequiredSection("Smtp");
            services.Configure<SmtpOptions>(smtpSection);

            services.AddHostedService<EmailSenderBackgroundService>();

            services.AddSingleton<IEmailSender, SmtpEmailSender>();

            //services
            //    .AddHealthChecks();

            return services;
        }
    }
}
