using Chat.Domain.Entities;
using Common.Blocks.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Chat.Infrastructure.Data.Contexts
{
    public class ChatDbContext(
        DbContextOptions<ChatDbContext> options) : CommonDbContext<ChatDbContext>(options)
    {
        public DbSet<BoardMessage> BoardMessages { get; set; }

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
