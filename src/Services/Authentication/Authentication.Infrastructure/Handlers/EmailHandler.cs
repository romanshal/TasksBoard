using Authentication.Domain.Interfaces.Handlers;
using EventBus.Messages.Abstraction.Events;
using MassTransit;

namespace Authentication.Infrastructure.Handlers
{
    internal class EmailHandler(
        IPublishEndpoint publishEndpoint) : IEmailHandler
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task HandleAsync(EmailMessageEvent message, CancellationToken cancellationToken = default)
        {
            await _publishEndpoint.Publish<EmailMessageEvent>(message, cancellationToken);
        }
    }
}
