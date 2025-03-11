using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Data.Contexts
{
    public class AuthenticationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
    }
}
