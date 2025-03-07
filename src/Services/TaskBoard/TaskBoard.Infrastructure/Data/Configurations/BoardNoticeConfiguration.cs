using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Data.Configurations
{
    public class BoardNoticeConfiguration : IEntityTypeConfiguration<BoardNotice>
    {
        public void Configure(EntityTypeBuilder<BoardNotice> builder)
        {
            builder.ToTable("boardnotices")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
