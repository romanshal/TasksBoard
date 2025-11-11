using EmailService.Infrastructure.Postgres.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailService.Infrastructure.Postgres.Data.Configurations
{
    internal sealed class EmailOutboxConfiguration : IEntityTypeConfiguration<EmailOutbox>
    {
        public void Configure(EntityTypeBuilder<EmailOutbox> builder)
        {
            builder
                .ToTable("email_outbox")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.MessageId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.To)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.From)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Subject)
                .HasMaxLength(1000);

            builder.Property(e => e.Body)
                .IsRequired();

            builder.Property(e => e.IsHtml)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.Attempts)
                .IsRequired();

            builder.Property(e => e.LastError).HasMaxLength(2000);

            builder.Ignore(i => i.Status);

            builder.HasIndex(e => e.MessageId).IsUnique();
            builder.HasIndex(i => new { i.StatusId, i.NextAttemptAt });

            builder
                .HasOne(o => o.EmailOutboxStatus)
                .WithMany(m => m.EmailOutboxes)
                .HasForeignKey(k => k.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
