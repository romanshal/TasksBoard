using Common.Blocks.Models.ApiResponses;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TasksBoard.API.Models.Requests.ManageBoardNotices;
using TasksBoard.Domain.Constants.Errors.DomainErrors;


namespace TasksBoard.Tests.E2E.Controllers
{
    public class ManageBoardNoticeControllerShould(
        TasksBoardApiApllicationFactory factory) : IClassFixture<TasksBoardApiApllicationFactory>
    {
        private readonly PreconfigurationDatabaseFactory _preconfig = new(factory);
        private readonly TasksBoardApiApllicationFactory _factory = factory;

        [Fact]
        public async Task CreateNewBoardNotice()
        {
            var boardId = await _preconfig.PreconfigureBoard();

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var request = new CreateBoardNoticeRequest
            {
                AuthorId = _preconfig.User.UserId,
                AuthorName = _preconfig.User.Username,
                Definition = "Test notice",
                BackgroundColor = "#606060",
                Rotation = "120deg"
            };

            var createNoticeResponse = await httpClient.PostAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));
            var responseString = await createNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, _factory.GetJsonSettings());

            createNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            createNoticeResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createNoticeResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ReturnForbbidenResponse_WhenHasntBoardAccess()
        {
            var boardId = Guid.NewGuid();

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var request = new CreateBoardNoticeRequest
            {
                AuthorId = _preconfig.User.UserId,
                AuthorName = _preconfig.User.Username,
                Definition = "Test notice",
                BackgroundColor = "#606060",
                Rotation = "120deg"
            };

            var createNoticeResponse = await httpClient.PostAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));
            var responseString = await createNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(responseString, _factory.GetJsonSettings());

            createNoticeResponse.IsSuccessStatusCode.Should().BeFalse();
            createNoticeResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateBoardNotice()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            var noticeId = await _preconfig.PreconfigureBoardNotice(boardId);

            var request = new UpdateBoardNoticeRequest
            {
                NoticeId = noticeId,
                Definition = "Test notice",
                BackgroundColor = "#606060",
                Rotation = "120deg"
            };

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var updateNoticeResponse = await httpClient.PutAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));
            var responseString = await updateNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, _factory.GetJsonSettings());

            updateNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            updateNoticeResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty().And.Be(noticeId);
        }

        [Fact]
        public async Task ReturnNotFoundResponse_WhenBoardNoticeNotFound_WhenUpdateBoardNotice()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            var noticeId = Guid.Parse("c4d77651-8057-48ea-8771-856b31d88b87");

            var request = new UpdateBoardNoticeRequest
            {
                NoticeId = noticeId,
                Definition = "Test notice",
                BackgroundColor = "#606060",
                Rotation = "120deg"
            };

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var updateNoticeResponse = await httpClient.PutAsync($"api/managenotices/board/{boardId}", JsonContent.Create(request));
            var responseString = await updateNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(responseString, _factory.GetJsonSettings());

            updateNoticeResponse.IsSuccessStatusCode.Should().BeFalse();
            updateNoticeResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.Description.Should().BeEquivalentTo(BoardNoticeErrors.NotFound.Description);
        }

        [Fact]
        public async Task UpdateBoardNoticeStatus()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            var noticeId = await _preconfig.PreconfigureBoardNotice(boardId);

            var request = new UpdateBoardNoticeStatusRequest
            {
                AccountId = _preconfig.User.UserId,
                AccountName = _preconfig.User.Username,
                NoticeId = noticeId,
                Complete = true
            };

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var updateNoticeStatusResponse = await httpClient.PutAsync($"api/managenotices/status/board/{boardId}", JsonContent.Create(request));
            var responseString = await updateNoticeStatusResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, _factory.GetJsonSettings());

            updateNoticeStatusResponse.IsSuccessStatusCode.Should().BeTrue();
            updateNoticeStatusResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty().And.Be(noticeId);
        }

        [Fact]
        public async Task ReturnNotFoundResponse_WhenBoardNoticeNotFound_WhenUpdateBoardNoticeStatus()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            var noticeId = Guid.Parse("c4d77651-8057-48ea-8771-856b31d88b87");

            var request = new UpdateBoardNoticeStatusRequest
            {
                AccountId = _preconfig.User.UserId,
                AccountName = _preconfig.User.Username,
                NoticeId = noticeId,
                Complete = true
            };

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var updateNoticeStatusResponse = await httpClient.PutAsync($"api/managenotices/status/board/{boardId}", JsonContent.Create(request));
            var responseString = await updateNoticeStatusResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(responseString, _factory.GetJsonSettings());

            updateNoticeStatusResponse.IsSuccessStatusCode.Should().BeFalse();
            updateNoticeStatusResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.Description.Should().BeEquivalentTo(BoardNoticeErrors.NotFound.Description);
        }

        [Fact]
        public async Task DeleteBoardNotice()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            var noticeId = await _preconfig.PreconfigureBoardNotice(boardId);

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var deleteNoticeResponse = await httpClient.DeleteAsync($"api/managenotices/board/{boardId}/notice/{noticeId}");
            var responseString = await deleteNoticeResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(responseString, _factory.GetJsonSettings());

            deleteNoticeResponse.IsSuccessStatusCode.Should().BeTrue();
            deleteNoticeResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
        }
    }
}
