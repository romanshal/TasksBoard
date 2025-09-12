using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    class BoardPermissionConfiguration : IEntityTypeConfiguration<BoardPermission>
    {
        public void Configure(EntityTypeBuilder<BoardPermission> builder)
        {
            builder
                .ToTable("boardpermissions")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id)
                .HasConversion(permId => permId.Value, dbId => BoardPermissionId.Of(dbId))
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");
        }
    }
}
