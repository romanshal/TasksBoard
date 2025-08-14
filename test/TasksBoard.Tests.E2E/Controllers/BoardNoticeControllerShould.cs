using Common.Blocks.Models;
using Common.Blocks.Models.ApiResponses;
using FluentAssertions;
using Newtonsoft.Json;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Tests.E2E.Controllers
{
    public class BoardNoticeControllerShould : IClassFixture<TasksBoardApiApllicationFactory>
    {
        private readonly PreconfigurationDatabaseFactory _preconfig;
        private readonly TasksBoardApiApllicationFactory _factory;

        public BoardNoticeControllerShould(TasksBoardApiApllicationFactory factory)
        {
            _factory = factory;
            _preconfig = new(factory);
        }

        [Fact]
        public async Task GetPaginatedBoardNoticesByBoardId()
        {
            var boardId = await _preconfig.PreconfigureBoard();
            await _preconfig.PreconfigureBoardNotice(boardId);

            using var httpClient = _factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = _factory.GetAuthentication();

            var getNoticesResponse = await httpClient.GetAsync($"api/boardnotices/board/{boardId}?pageIndex=1&pageSize=10");
            var responseString = await getNoticesResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse<PaginatedList<BoardNoticeDto>>>(
                responseString, 
                _factory.GetJsonSettings());

            getNoticesResponse.IsSuccessStatusCode.Should().BeTrue();
            getNoticesResponse.Content.Should().NotBeNull();
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Result.Items.Should().HaveCount(1);
            response.Result.TotalCount.Should().Be(1);
        }
    }
}
