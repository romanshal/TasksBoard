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
                        Description = "Read only access",
                        AccessLevel = 0
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_notice",
                        Description = "Create, update and delete board notices",
                        AccessLevel = 1
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_chat",
                        Description = "Chat administration access",
                        AccessLevel = 2
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_member",
                        Description = "Invite, manage and delete members",
                        AccessLevel = 3
                    },
                    new BoardPermission
                    {
                        Id = Guid.NewGuid(),
                        Name = "manage_board",
                        Description = "Update board info",
                        AccessLevel = 4
                    }
                ];
        }
    }
}
