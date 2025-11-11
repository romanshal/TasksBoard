using MimeKit.Utils;

namespace EmailService.Core.Models
{
    public class EmailMessage
    {
        //public string MessageId { get; init; } = Guid.NewGuid().ToString("N");
        public string MessageId { get; init; } = MimeUtils.GenerateMessageId();
        public string To { get; init; } = default!;
        public string From { get; init; } = default!;
        public string Subject { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public bool IsHtml { get; init; } = true;
    }
}
