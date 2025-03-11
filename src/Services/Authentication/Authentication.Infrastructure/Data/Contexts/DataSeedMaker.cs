using Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Authentication.Infrastructure.Data.Contexts
{
    public class DataSeedMaker
    {
        public static async Task SeedAsync(AuthenticationDbContext context, ILogger<DataSeedMaker> logger)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(GetPreconfiguredRoles());
                await context.SaveChangesAsync();
                logger.LogInformation($"Seed database associated with context {nameof(AuthenticationDbContext)}");
            }
        }

        private static IEnumerable<ApplicationRole> GetPreconfiguredRoles()
        {
            return
            [
                new ApplicationRole
                {
                    Id = Guid.Parse("d341ddbc-c3b4-4b3f-b175-72f5a958c091"),
                    Name = "user",
                    NormalizedName = "USER",
                },

                new ApplicationRole
                {
                    Id = Guid.Parse("63a5e091-28f3-4269-9abc-446bb4683f02"),
                    Name = "admin",
                    NormalizedName = "ADMIN",
                }
            ];
        }
    }
}
