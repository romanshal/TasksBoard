using Common.Blocks.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Blocks.Contexts
{
    public abstract class CommonDbContext<TContext>(
        DbContextOptions<TContext> options) : DbContext(options) where TContext : DbContext
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
