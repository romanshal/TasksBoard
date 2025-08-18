using Microsoft.EntityFrameworkCore;
using TasksBoard.Domain.Entities;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Tests.E2E
{
    public class PreconfigurationDatabaseFactory(TasksBoardApiApllicationFactory factory)
    {
        public readonly (Guid UserId, string Username) User = factory.GetUserCredentials();

        public async Task<Guid> PreconfigureBoard()
        {
            var dbContext = factory.GetDbContext();

            var permissionsExist = await dbContext.BoardPermissions.AnyAsync();
            if (!permissionsExist)
            {
                await dbContext.BoardPermissions.AddRangeAsync(DataSeedMaker.GetPreconfiguredPermissions());
                await dbContext.SaveChangesAsync();
            }
            var permissions = await dbContext.BoardPermissions.ToListAsync();

            var board = new Board
            {
                Name = "Test board",
                OwnerId = User.UserId,
                BoardMembers =
                [
                    new() {
                        AccountId = User.UserId,
                        BoardMemberPermissions = [.. permissions.Select(perm => new BoardMemberPermission
                        {
                            BoardPermissionId = perm.Id
                        })]
                    }
                ]
            };

            dbContext.Boards.Add(board);

            await dbContext.SaveChangesAsync();

            return board.Id;
        }

        public async Task<Guid> PreconfigureBoardNotice(Guid boardId)
        {
            var dbContext = factory.GetDbContext();

            var notice = new BoardNotice
            {
                BoardId = boardId,
                AuthorId = User.UserId,
                //AuthorName = User.Username,
                Definition = "Test notice",
                BackgroundColor = "BackgroundColor",
                Rotation = "Rotation"
            };

            dbContext.BoardNotices.Add(notice);
            await dbContext.SaveChangesAsync();

            return notice.Id;
        }
    }
}
