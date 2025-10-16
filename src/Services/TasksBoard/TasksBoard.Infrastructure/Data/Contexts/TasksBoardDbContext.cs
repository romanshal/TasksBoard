using Common.Blocks.Contexts;
using Common.Outbox.Abstraction.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Contexts
{
    public class TasksBoardDbContext(
        DbContextOptions<TasksBoardDbContext> options) : CommonDbContext<TasksBoardDbContext>(options)
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

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(TasksBoardDbContext))!);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(OutboxEvent))!);
        }
    }
}
