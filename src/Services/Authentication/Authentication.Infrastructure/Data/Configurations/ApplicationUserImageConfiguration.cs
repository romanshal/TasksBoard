using Authentication.Domain.Entities;
using Authentication.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Infrastructure.Data.Configurations
{
    public class ApplicationUserImageConfiguration : IEntityTypeConfiguration<ApplicationUserImage>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserImage> builder)
        {
            builder
                .ToTable("userimages")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(imageId => imageId.Value, dbId => ImageId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder
                .HasOne(o => o.User)
                .WithOne(o => o.Image)
                .HasForeignKey<ApplicationUserImage>(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
