using Common.Blocks.Contexts;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using System.Reflection;

namespace Notification.Infrastructure.Data.Contexts
{
    public class NotificationDbContext(
        DbContextOptions<NotificationDbContext> options) : CommonDbContext<NotificationDbContext>(options)
    {
        public DbSet<ApplicationEvent> ApplicationEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
