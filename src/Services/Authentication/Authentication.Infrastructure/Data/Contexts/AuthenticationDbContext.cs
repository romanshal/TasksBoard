using Authentication.Domain.Entities;
using Common.Blocks.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Authentication.Infrastructure.Data.Contexts
{
    public class AuthenticationDbContext(
        DbContextOptions<AuthenticationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        public DbSet<ApplicationUserImage> ApplicationUserImages { get; set; }
        public DbSet<ApplicationUserSession> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<ApplicationUser>(entity => entity.ToTable(name: "users"));
            builder.Entity<ApplicationRole>(entity => entity.ToTable(name: "roles"));
            builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable(name: "userroles"));
            builder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable(name: "userclaim"));
            builder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("userlogins"));
            builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("usertokens"));
            builder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("roleclaims"));

            var userRole = new ApplicationRole
            {
                Id = Guid.Parse("d341ddbc-c3b4-4b3f-b175-72f5a958c091"),
                Name = "user",
                NormalizedName = "USER",
            };

            var adminRole = new ApplicationRole
            {
                Id = Guid.Parse("63a5e091-28f3-4269-9abc-446bb4683f02"),
                Name = "admin",
                NormalizedName = "ADMIN",
            };

            builder.Entity<ApplicationRole>().HasData(userRole, adminRole);
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
