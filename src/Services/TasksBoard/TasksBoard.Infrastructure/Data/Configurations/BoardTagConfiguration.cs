using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardTagConfiguration : IEntityTypeConfiguration<BoardTag>
    {
        public void Configure(EntityTypeBuilder<BoardTag> builder)
        {
            builder.ToTable("boardtags")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasIndex(p => new
            {
                p.BoardId,
                p.Tag
            })
                .IsUnique();

            builder.HasOne(o => o.Board)
                .WithMany(m => m.Tags)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
