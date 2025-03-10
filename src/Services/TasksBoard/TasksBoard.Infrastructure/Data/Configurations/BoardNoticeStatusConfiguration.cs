using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardNoticeStatusConfiguration : IEntityTypeConfiguration<BoardNoticeStatus>
    {
        public void Configure(EntityTypeBuilder<BoardNoticeStatus> builder)
        {
            builder.ToTable("boardnoticestatuses")
                .HasKey(k => k.Id);

            builder.HasIndex(i => i.Name).IsUnique();
        }
    }
}
