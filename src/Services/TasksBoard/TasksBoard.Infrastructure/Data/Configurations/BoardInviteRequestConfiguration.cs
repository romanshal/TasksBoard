using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardInviteRequestConfiguration : IEntityTypeConfiguration<BoardInviteRequest>
    {
        public void Configure(EntityTypeBuilder<BoardInviteRequest> builder)
        {
            builder
                .ToTable("boardinviterequests")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(inviteId => inviteId.Value, dbId => BoardInviteId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .HasConversion(id => id.Value, value => BoardId.Of(value))
                .HasColumnName("BoardId")
                .IsRequired();

            builder
                .Property(p => p.FromAccountId)
                .HasConversion(id => id.Value, value => AccountId.Of(value))
                .HasColumnName("FromAccountId")
                .IsRequired();

            builder
                .Property(p => p.ToAccountId)
                .HasConversion(id => id.Value, value => AccountId.Of(value))
                .HasColumnName("ToAccountId")
                .IsRequired();
        }
    }
}
