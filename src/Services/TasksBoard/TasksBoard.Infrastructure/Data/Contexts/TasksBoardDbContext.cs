﻿using Common.Blocks.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Contexts
{
    public class TasksBoardDbContext(DbContextOptions<TasksBoardDbContext> options) : DbContext(options)
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<BoardNotice> BoardNotices { get; set; }
        public DbSet<BoardMember> BoardMembers { get; set; }
        public DbSet<BoardPermission> BoardPermissions { get; set; }
        public DbSet<BoardMemberPermission> BoardMemberPermissions { get; set; }
        public DbSet<BoardTag> BoardTags { get; set; }
        public DbSet<BoardAccessRequest> BoardAccessRequests { get; set; }
        public DbSet<BoardInviteRequest> BoardInviteRequests { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(OutboxEvent))!);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
