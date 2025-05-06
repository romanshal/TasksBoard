using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardImageConfiguration : IEntityTypeConfiguration<BoardImage>
    {
        public void Configure(EntityTypeBuilder<BoardImage> builder)
        {
            builder.ToTable("boardimages")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasOne(o => o.Board)
                .WithOne(o => o.BoardImage)
                .HasForeignKey<BoardImage>(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
