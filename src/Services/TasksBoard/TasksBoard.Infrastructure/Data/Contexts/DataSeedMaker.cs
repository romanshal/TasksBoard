using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Contexts
{
    public class DataSeedMaker
    {
        public static async Task SeedAsync(TasksBoardDbContext context, ILogger<DataSeedMaker> logger)
        {
            if (!context.BoardPermissions.Any())
            {
                context.BoardPermissions.AddRange(GetPreconfiguredPermissions());
                await context.SaveChangesAsync();
                logger.LogInformation($"Seed database associated with context {nameof(TasksBoardDbContext)}");
            }
        }

        private static IEnumerable<BoardPermission> GetPreconfiguredPermissions()
        {
            return
                [
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "read",
                        AccessLevel = 0
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_notice",
                        AccessLevel = 1
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_chat",
                        AccessLevel = 2
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_member",
                        AccessLevel = 3
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_board",
                        AccessLevel = 4
                    }
                ];
        }
    }
}
