using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Factories;
using Authentication.Infrastructure.Options;
using EventBus.Messages.Abstraction.Events;
using Microsoft.Extensions.Options;

namespace Authentication.Infrastructure.Factories
{
    internal class EmailMessageFactory(IOptions<FrontendUrlsOption> option) : IEmailMessageFactory
    {
        private readonly FrontendUrlsOption _urls = option.Value;

        public EmailMessageEvent Create(EmailType type, ApplicationUser user, string token)
        {
            return type switch
            {
                EmailType.ConfirmEmail => new EmailMessageEvent
                {
                    Recipient = user.Email!,
                    Subject = "Confirm email",
                    Body = EmailTexts.ConfirmEmail(_urls.ConfirmEmail, user.Id, token)
                },
                EmailType.ResetPassword => new EmailMessageEvent
                {
                    Recipient = user.Email!,
                    Subject = "Reset password",
                    Body = EmailTexts.ResetPassword(_urls.ConfirmEmail, user.Id, token)
                },
                EmailType.TwoFactor => new EmailMessageEvent
                {
                    Recipient = user.Email!,
                    Subject = "",
                    Body = EmailTexts.TwoFactor(_urls.ConfirmEmail, user.Id, token)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
