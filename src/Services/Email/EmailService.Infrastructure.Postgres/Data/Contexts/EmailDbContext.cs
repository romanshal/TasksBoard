using Common.Blocks.Contexts;
using EmailService.Core.Constants;
using EmailService.Infrastructure.Postgres.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EmailService.Infrastructure.Postgres.Data.Contexts
{
    public class EmailDbContext(
        DbContextOptions<EmailDbContext> options) : CommonDbContext<EmailDbContext>(options)
    {
        public DbSet<EmailOutbox> EmailOutboxes { get; set; }
        public DbSet<EmailOutboxStatus> EmailOutboxStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(EmailDbContext))!);
        }
    }
}
