using MimeKit.Utils;

namespace EventBus.Messages.Abstraction.Events
{
    public class EmailMessageEvent
    {
        //public string MessageId { get; init; } = Guid.NewGuid().ToString("N");
        public string MessageId { get; init; } = MimeUtils.GenerateMessageId();
        public string Recipient { get; init; } = default!;
        public string Sender { get; init; } = default!;
        public string Subject { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public bool IsHtml { get; init; } = true;
    }
}
