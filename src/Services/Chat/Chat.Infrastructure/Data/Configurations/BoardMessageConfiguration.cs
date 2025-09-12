using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Data.Configurations
{
    public class BoardMessageConfiguration : IEntityTypeConfiguration<BoardMessage>
    {
        public void Configure(EntityTypeBuilder<BoardMessage> builder)
        {
            builder
                .ToTable("boardmessages")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(messageId => messageId.Value, dbId => MessageId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .IsRequired();

            builder
                .Property(p => p.MemberId)
                .IsRequired();

            builder
                .HasIndex(i => i.BoardId);

            builder
                .HasIndex(i => new { i.BoardId, i.MemberId });

        }
    }
}
