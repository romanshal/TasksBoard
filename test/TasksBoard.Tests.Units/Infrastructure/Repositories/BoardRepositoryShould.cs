using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using TasksBoard.Domain.Entities;
using TasksBoard.Infrastructure.Data.Contexts;
using TasksBoard.Infrastructure.Repositories;

namespace TasksBoard.Tests.Units.Infrastructure.Repositories
{
    public class BoardRepositoryShould : IClassFixture<InfrastructureTestFixture>
    {
        private readonly BoardRepository _sut;
        private readonly TasksBoardDbContext _dbContext;
        private readonly InfrastructureTestFixture _fixture;

        public BoardRepositoryShould(InfrastructureTestFixture fixture)
        {
            this._fixture = fixture;

            var loggerFactory = new Mock<ILoggerFactory>();

            _dbContext = fixture.GetDbContext();

            _sut = new BoardRepository(_dbContext, loggerFactory.Object);
        }

        [Fact]
        public async Task CreateNewBoard()
        {
            var board = new Board
            {
                OwnerId = AccountId.Of("fe6e93de-5599-4f32-a143-4a4da06e6cd3"),
                Name = "Test name"
            };

            _sut.Add(board);
            var actual = await _dbContext.SaveChangesAsync();

            await using var localDbContext = _fixture.GetDbContext();
            var boards = await localDbContext.Boards.ToArrayAsync();

            actual.Should().Be(1);
            boards.Should().HaveCount(1);
        }
    }
}
