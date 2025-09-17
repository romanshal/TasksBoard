using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardNoticeConfiguration : IEntityTypeConfiguration<BoardNotice>
    {
        public void Configure(EntityTypeBuilder<BoardNotice> builder)
        {
            builder
                .ToTable("boardnotices")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(noticeId => noticeId.Value, dbId => BoardNoticeId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .Property(p => p.BoardId)
                .HasConversion(id => id.Value, value => BoardId.Of(value))
                .HasColumnName("BoardId")
                .IsRequired();

            builder
                .Property(p => p.AuthorId)
                .HasConversion(id => id.Value, value => AccountId.Of(value))
                .HasColumnName("AuthorId")
                .IsRequired();
        }
    }
}
