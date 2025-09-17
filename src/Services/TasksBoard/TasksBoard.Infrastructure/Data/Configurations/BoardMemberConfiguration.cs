using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
    {
        public void Configure(EntityTypeBuilder<BoardMember> builder)
        {
            builder
                .ToTable("boardmembers")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(memberId => memberId.Value, dbId => BoardMemberId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .HasConversion(id => id.Value, value => BoardId.Of(value))
                .HasColumnName("BoardId")
                .IsRequired();

            builder
                .Property(p => p.AccountId)
                .HasConversion(id => id.Value, value => AccountId.Of(value))
                .HasColumnName("AccountId")
                .IsRequired();

            builder
                .HasIndex(p => new { p.BoardId, p.AccountId })
                .IsUnique();
        }
    }
}
