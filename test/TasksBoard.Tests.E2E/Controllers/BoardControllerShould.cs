using Common.Blocks.Models.ApiResponses;
using FluentAssertions;
using Newtonsoft.Json;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Tests.E2E.Controllers
{
    public class BoardControllerShould(
        TasksBoardApiApllicationFactory factory) : IClassFixture<TasksBoardApiApllicationFactory>
    {
        [Fact]
        public async Task CreateNewBoard()
        {
            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            var boardName = "Test name";
            var (userId, username) = factory.GetUserCredentials();
            var form = new FormUrlEncodedContent(
            [
                new("OwnerId", userId.ToString()),
                new("OwnerNickname", username),
                new("Name", boardName),
                new("Description", "Test description"),
                new("Public", "true")
            ]);

            using var createBoardResponse = await httpClient.PostAsync("api/boards", form);

            createBoardResponse.IsSuccessStatusCode.Should().BeTrue();
            createBoardResponse.Content.Should().NotBeNull();

            var responseString = await createBoardResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<Guid>>(responseString, factory.GetJsonSettings());

            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Should().NotBeEmpty();

            using var getCreatedBoardResponse = await httpClient.GetAsync($"api/boards/{response.Result}");

            getCreatedBoardResponse.IsSuccessStatusCode.Should().BeTrue();
            getCreatedBoardResponse.Content.Should().NotBeNull();

            var getResponseString = await getCreatedBoardResponse.Content.ReadAsStringAsync();
            var getResponse = JsonConvert.DeserializeObject<ApiResponse<BoardDto>>(getResponseString, factory.GetJsonSettings());

            getResponse.Should().NotBeNull();
            getResponse.IsError.Should().BeFalse();
            getResponse.Result.Id.Should().Be(response.Result);
            getResponse.Result.Name.Should().Be(boardName);
        }

        [Fact]
        public async Task GetPublicBoards()
        {
            using var httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = factory.GetAuthentication();

            using var publicBoardsResponse = await httpClient.GetAsync("api/boards/public?pageIndex=1&pageSize=10");

            publicBoardsResponse.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
