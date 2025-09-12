using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardImageConfiguration : IEntityTypeConfiguration<BoardImage>
    {
        public void Configure(EntityTypeBuilder<BoardImage> builder)
        {
            builder
                .ToTable("boardimages")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(imageId => imageId.Value, dbId => BoardImageId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .HasConversion(id => id.Value, value => BoardId.Of(value))
                .HasColumnName("BoardId")
                .IsRequired();

            builder
                .HasOne(o => o.Board)
                .WithOne(o => o.BoardImage)
                .HasForeignKey<BoardImage>(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
