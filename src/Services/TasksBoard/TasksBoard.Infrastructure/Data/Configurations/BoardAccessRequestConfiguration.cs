using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardAccessRequestConfiguration : IEntityTypeConfiguration<BoardAccessRequest>
    {
        public void Configure(EntityTypeBuilder<BoardAccessRequest> builder)
        {
            builder.ToTable("boardaccessrequests")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();


            builder.HasOne(o => o.Board)
                .WithMany(m => m.AccessRequests)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
