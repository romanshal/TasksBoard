using Chat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Data.Configurations
{
    public class BoardMessageConfiguration : IEntityTypeConfiguration<BoardMessage>
    {
        public void Configure(EntityTypeBuilder<BoardMessage> builder)
        {
            builder.ToTable("boardmessages");

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.BoardId).IsRequired();

            builder.Property(p => p.MemberId).IsRequired();

            builder.HasIndex(i => i.BoardId);

            builder.HasIndex(i => new { i.BoardId, i.MemberId });

        }
    }
}
