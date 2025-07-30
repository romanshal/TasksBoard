using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoards
{
    public class UpdateBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<UpdateBoardCommandHandler>> logger;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly UpdateBoardCommandHandler sut;

        public UpdateBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<UpdateBoardCommandHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);

            sut = new UpdateBoardCommandHandler(logger.Object, unitOfWork.Object);
        }

        [Fact]
        public async Task ReturnUpdatedBoardId_WhenBoardUpdated()
        {
            var boardId = Guid.Parse("7221344f-5865-495b-acfe-dcf2f286f1cb");
            var command = new UpdateBoardCommand
            {
                BoardId = boardId,
                Name = string.Empty,
                Tags = []
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = boardId,
                    OwnerId = Guid.Empty,
                    Name = string.Empty,
                    Tags = []
                });

            boardRepository.Setup(s => s.Update(It.IsAny<Board>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(boardId);

            boardRepository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            boardRepository.Verify(s => s.Update(It.IsAny<Board>()), Times.Once);
            boardRepository.VerifyNoOtherCalls();

            unitOfWork.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            boardRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("7221344f-5865-495b-acfe-dcf2f286f1cb");
            var command = new UpdateBoardCommand
            {
                BoardId = boardId,
                Name = string.Empty,
                Tags = []
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            //await sut
            //    .Invoking(s => s.Handle(command, CancellationToken.None))
            //    .Should()
            //    .ThrowAsync<NotFoundException>()
            //    .WithMessage($"Board with id '{boardId}' not found.");

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);

            boardRepository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            boardRepository.VerifyNoOtherCalls();
        }
    }
}
