using EmailService.Core.Constants;
using EventBus.Messages.Abstraction.Events;

namespace EmailService.Infrastructure.Postgres.Data.Entities
{
    public class EmailOutbox : EmailMessageEvent
    {
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public int Attempts { get; init; } = 0;
        public DateTimeOffset? NextAttemptAt { get; init; } = DateTimeOffset.UtcNow;
        public OutboxStatuses Status
        {
            get => (OutboxStatuses)StatusId;
            set => StatusId = (int)value;
        }
        public int StatusId { get; set; }
        public string? LastError { get; init; }

        public virtual EmailOutboxStatus EmailOutboxStatus { get; init; } = default!;
    }
}
