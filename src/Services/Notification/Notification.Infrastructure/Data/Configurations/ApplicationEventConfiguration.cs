using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Chat.Infrastructure.Data.Configurations
{
    public class ApplicationEventConfiguration : IEntityTypeConfiguration<ApplicationEvent>
    {
        public void Configure(EntityTypeBuilder<ApplicationEvent> builder)
        {
            builder.ToTable("events");

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasIndex(i => i.EventId);

            builder.HasIndex(e => e.EventType);

            builder.Property(e => e.EventType).IsRequired().HasMaxLength(100);

            builder.Property(p => p.Payload).IsRequired().HasColumnType("jsonb");
        }
    }
}
