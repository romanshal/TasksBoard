using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardNoticeConfiguration : IEntityTypeConfiguration<BoardNotice>
    {
        public void Configure(EntityTypeBuilder<BoardNotice> builder)
        {
            builder.ToTable("boardnotices")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasOne(o => o.BoardNoticeStatus)
                .WithMany(m => m.BoardNotices)
                .HasForeignKey(k => k.NoticeStatusId);
        }
    }
}
