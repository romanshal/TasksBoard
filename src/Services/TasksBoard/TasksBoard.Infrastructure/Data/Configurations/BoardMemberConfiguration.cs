using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
    {
        public void Configure(EntityTypeBuilder<BoardMember> builder)
        {
            builder.ToTable("boardmembers")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Nickname).IsRequired().HasMaxLength(64).HasDefaultValue("default");

            builder.HasIndex(p => new
            {
                p.BoardId,
                p.AccountId
            })
                .IsUnique();
        }
    }
}
