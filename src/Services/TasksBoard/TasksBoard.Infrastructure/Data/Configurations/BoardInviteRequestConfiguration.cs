﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Configurations
{
    public class BoardInviteRequestConfiguration : IEntityTypeConfiguration<BoardInviteRequest>
    {
        public void Configure(EntityTypeBuilder<BoardInviteRequest> builder)
        {
            builder.ToTable("boardinviterequests")
                .HasKey(k => k.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();


            builder.HasOne(o => o.Board)
                .WithMany(m => m.InviteRequests)
                .HasForeignKey(k => k.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
