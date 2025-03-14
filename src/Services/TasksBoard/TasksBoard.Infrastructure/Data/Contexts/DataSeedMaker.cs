using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Infrastructure.Data.Contexts
{
    public class DataSeedMaker
    {
        public static async Task SeedAsync(TasksBoardDbContext context, ILogger<DataSeedMaker> logger)
        {
            if (!context.BoardNoticeStatuses.Any())
            {
                context.BoardNoticeStatuses.AddRange(GetPreconfiguredNoticeStatuses());
                await context.SaveChangesAsync();
                logger.LogInformation($"Seed database associated with context {nameof(TasksBoardDbContext)}");
            }

            if (!context.BoardPermissions.Any())
            {
                context.BoardPermissions.AddRange(GetPreconfiguredPermissions());
                await context.SaveChangesAsync();
                logger.LogInformation($"Seed database associated with context {nameof(TasksBoardDbContext)}");
            }
        }

        private static IEnumerable<BoardNoticeStatus> GetPreconfiguredNoticeStatuses()
        {
            return
                [
                    new BoardNoticeStatus
                    {
                        Id = Guid.Parse("a3372135-ea3d-4eb9-8209-5a36634b2bba"),
                        Name = "New"
                    },
                    new BoardNoticeStatus
                    {
                        Id = Guid.Parse("389ae405-a369-4e6d-b5ee-a8eacdbd239e"),
                        Name = "Canceled"
                    },
                    new BoardNoticeStatus
                    {
                        Id = Guid.Parse("4eb7a9b9-0f08-4425-8978-04a66f006882"),
                        Name = "Completed"
                    }
                ];
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
                        Name = "manage_notices",
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
                        Name = "manage_members",
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
