using FluentAssertions;
using System.Net.Http.Json;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;

namespace TasksBoard.Tests.E2E.Controllers
{
    public class BoardControllerShould(TasksBoardApiApllicationFactory factory) : IClassFixture<TasksBoardApiApllicationFactory>
    {
        //[Fact]
        public async Task CreateNewBoard()
        {
            using var httpClient = factory.CreateClient();

            var board = new CreateBoardCommand
            {
                OwnerId = Guid.Parse("56114028-b282-45b3-b6ca-89982965e4c8"),
                OwnerNickname = "Owner nickname",
                Name = "Test name",
                Description = "Test description",
                Public = true,
            };

            using var createBoardResponse = await httpClient.PostAsync("api/boards", JsonContent.Create(board));

            createBoardResponse.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
