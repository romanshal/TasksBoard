using EmailService.Core.Constants;
using EmailService.Infrastructure.Postgres.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailService.Infrastructure.Postgres.Data.Configurations
{
    internal sealed class EmailOutboxStatusConfiguration : IEntityTypeConfiguration<EmailOutboxStatus>
    {
        public void Configure(EntityTypeBuilder<EmailOutboxStatus> builder)
        {
            builder
                .ToTable("outbox_statuses")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Name).HasMaxLength(24).IsRequired();

            builder
                .HasData(Enum.GetValues<OutboxStatuses>()
                .Select(e => new EmailOutboxStatus
                {
                    Id = (int)e,
                    Name = e.ToString()
                }));
        }
    }
}
