namespace EmailService.Infrastructure.Postgres.Data.Entities
{
    public class EmailOutboxStatus
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public virtual ICollection<EmailOutbox> EmailOutboxes { get; set; }
    }
}
