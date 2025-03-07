using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardMemberPermissionConfiguration : IEntityTypeConfiguration<BoardMemberPermission>
    {
        public void Configure(EntityTypeBuilder<BoardMemberPermission> builder)
        {
            builder.ToTable("boardmemberspermissions")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasOne(o => o.BoardMember)
                .WithMany(m => m.BoardMemberPermissions)
                .HasForeignKey(k => k.BoardMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.BoardPermission)
                .WithMany(m => m.BoardMemberPermissions)
                .HasForeignKey(k => k.BoardPermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
