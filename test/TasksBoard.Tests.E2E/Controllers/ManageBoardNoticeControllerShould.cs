using Common.Blocks.Models.ApiResponses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Json;
using TasksBoard.API.Models.Requests.ManageBoardNotices;
using TasksBoard.Domain.Entities;
using TasksBoard.Infrastructure.Data.Contexts;


namespace TasksBoard.Tests.E2E.Controllers
{
    public class ManageBoardNoticeControllerShould(
        TasksBoardApiApllicationFactory factory) : IClassFixture<TasksBoardApiApllicationFactory>
    {
        private readonly (Guid UserId, string Username) _user = factory.GetUserCredentials();

        [Fact]
        public async Task CreateNewBoardNotice()
        {
            var boardId = await PreconfigureBoard();

            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            var request = new CreateBoardNoticeRequest 
            {
                AuthorId = _user.UserId,
                AuthorName = _user.Username,
                Definition = "Test notice",
                BackgroundColor = "BackgroundColor",
                Rotation = "Rotation"
            };

            var createNoticeResponse = await httpClient.PostAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));

            createNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            createNoticeResponse.Content.Should().NotBeNull();

            var responseString = await createNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, factory.GetJsonSettings());

            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateBoardNotice()
        {
            var boardId = await PreconfigureBoard();
            var noticeId = await PreconfigureBoardNotice(boardId);
            
            var request = new UpdateBoardNoticeRequest 
            {
                NoticeId = noticeId,
                Definition = "Test notice",
                BackgroundColor = "BackgroundColor",
                Rotation = "Rotation"
            };

            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            var updateNoticeResponse = await httpClient.PutAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));

            updateNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            updateNoticeResponse.Content.Should().NotBeNull();

            var responseString = await updateNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, factory.GetJsonSettings());

            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty().And.Be(noticeId);
        }

        [Fact]
        public async Task UpdateBoardNoticeStatus()
        {
            var boardId = await PreconfigureBoard();
            var noticeId = await PreconfigureBoardNotice(boardId);

            var request = new UpdateBoardNoticeStatusRequest
            {
                AccountId = _user.UserId,
                AccountName = _user.Username,
                NoticeId = noticeId,
                Complete = true
            };

            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            var updateNoticeStatusResponse = await httpClient.PutAsync($"api/managenotices/status/board/{boardId}", JsonContent.Create(request));

            updateNoticeStatusResponse.IsSuccessStatusCode.Should().BeTrue();
            updateNoticeStatusResponse.Content.Should().NotBeNull();

            var responseString = await updateNoticeStatusResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, factory.GetJsonSettings());

            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty().And.Be(noticeId);
        }

        [Fact]
        public async Task DeleteBoardNotice()
        {
            var boardId = await PreconfigureBoard();
            var noticeId = await PreconfigureBoardNotice(boardId);

            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            var deleteNoticeResponse = await httpClient.DeleteAsync($"api/managenotices/board/{boardId}/notice/{noticeId}");

            deleteNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            deleteNoticeResponse.Content.Should().NotBeNull();

            var responseString = await deleteNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(responseString, factory.GetJsonSettings());

            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
        }

        private async Task<Guid> PreconfigureBoard()
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
                OwnerId = _user.UserId,
                BoardMembers =
                [
                    new() {
                        AccountId = _user.UserId,
                        Nickname = _user.Username,
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

        private async Task<Guid> PreconfigureBoardNotice(Guid boardId)
        {
            var dbContext = factory.GetDbContext();

            var notice = new BoardNotice
            {
                BoardId = boardId,
                AuthorId = _user.UserId,
                AuthorName = _user.Username,
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
