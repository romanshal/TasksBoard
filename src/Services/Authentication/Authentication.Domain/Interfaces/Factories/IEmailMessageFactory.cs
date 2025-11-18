using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Entities;
using EventBus.Messages.Abstraction.Events;

namespace Authentication.Domain.Interfaces.Factories
{
    public interface IEmailMessageFactory
    {
        EmailMessageEvent Create(EmailType type, ApplicationUser user, string token);
    }
}
