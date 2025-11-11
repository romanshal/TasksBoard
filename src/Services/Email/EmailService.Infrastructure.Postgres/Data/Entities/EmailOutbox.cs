using EmailService.Core.Constants;
using EmailService.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailService.Infrastructure.Postgres.Data.Entities
{
    public class EmailOutbox : EmailMessage
    {
        public Guid Id { get; init; }
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
