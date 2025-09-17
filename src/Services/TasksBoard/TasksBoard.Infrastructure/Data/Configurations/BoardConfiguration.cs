using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder
                .ToTable("boards")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(boardId => boardId.Value, dbId => BoardId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .HasMany(m => m.BoardNotices)
                .WithOne(o => o.Board)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.BoardMembers)
                .WithOne(o => o.Board)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.BoardAccessRequests)
                .WithOne(o => o.Board)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.BoardInviteRequests)
                .WithOne(o => o.Board)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.BoardTags)
                .WithOne(o => o.Board)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
