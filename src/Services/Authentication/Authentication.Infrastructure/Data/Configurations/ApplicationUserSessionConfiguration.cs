using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Infrastructure.Data.Configurations
{
    public class ApplicationUserSessionConfiguration : IEntityTypeConfiguration<ApplicationUserSession>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserSession> builder)
        {
            builder
                .ToTable("usersessions")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(o => o.User)
                .WithMany(m => m.RefreshTokens)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(o => o.ReplacedBySession)
                .WithOne(o => o.ReplaceSession)
                .HasForeignKey<ApplicationUserSession>(k => k.ReplacedBySessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasIndex(i => new { i.UserId, i.RefreshTokenHash })
                .IsUnique();
        }
    }
}
