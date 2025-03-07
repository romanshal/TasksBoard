using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoard.Infrastructure.Entities;

namespace TaskBoard.Infrastructure.Configurations
{
    public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
    {
        public void Configure(EntityTypeBuilder<BoardMember> builder)
        {
            builder.ToTable("boardmembers")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
