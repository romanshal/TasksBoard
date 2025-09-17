using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;
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
                Id = BoardId.New(),
                Name = "Test board",
                OwnerId = AccountId.Of(User.UserId)
            };

            board.BoardMembers =
                [
                    new()
                    {
                        Id = BoardMemberId.New(),
                        BoardId = board.Id,
                        AccountId = AccountId.Of(User.UserId),
                        BoardMemberPermissions = [.. permissions.Select(perm => new BoardMemberPermission
                        {
                            BoardPermissionId = perm.Id
                        })]
                    }
                ];

            dbContext.Boards.Add(board);

            await dbContext.SaveChangesAsync();

            return board.Id.Value;
        }

        public async Task<Guid> PreconfigureBoardNotice(Guid boardId)
        {
            var dbContext = factory.GetDbContext();

            var notice = new BoardNotice
            {
                BoardId = BoardId.Of(boardId),
                AuthorId = AccountId.Of(User.UserId),
                //AuthorName = User.Username,
                Definition = "Test notice",
                BackgroundColor = "BackgroundColor",
                Rotation = "Rotation"
            };

            dbContext.BoardNotices.Add(notice);
            await dbContext.SaveChangesAsync();

            return notice.Id.Value;
        }
    }
}
