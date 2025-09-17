using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardAccessRequestConfiguration : IEntityTypeConfiguration<BoardAccessRequest>
    {
        public void Configure(EntityTypeBuilder<BoardAccessRequest> builder)
        {
            builder
                .ToTable("boardaccessrequests")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(accessId => accessId.Value, dbId => BoardAccessId.Of(dbId))
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
        }
    }
}
