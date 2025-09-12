using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardTagConfiguration : IEntityTypeConfiguration<BoardTag>
    {
        public void Configure(EntityTypeBuilder<BoardTag> builder)
        {
            builder
                .ToTable("boardtags")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(tagId => tagId.Value, dbId => BoardTagId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .HasConversion(id => id.Value, value => BoardId.Of(value))
                .HasColumnName("BoardId")
                .IsRequired();

            builder
                .HasIndex(p => new { p.BoardId, p.Tag })
                .IsUnique();
        }
    }
}
