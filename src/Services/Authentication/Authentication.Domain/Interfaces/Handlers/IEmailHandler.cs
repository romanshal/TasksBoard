using EventBus.Messages.Abstraction.Events;

namespace Authentication.Domain.Interfaces.Handlers
{
    public interface IEmailHandler
    {
        Task HandleAsync(EmailMessageEvent message, CancellationToken cancellationToken = default);
    }
}
