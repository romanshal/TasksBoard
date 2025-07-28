using Common.Blocks.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoards
{
    public class DeleteBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<DeleteBoardCommandHandler>> logger;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly DeleteBoardCommandHandler sut;

        public DeleteBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<DeleteBoardCommandHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);

            sut = new DeleteBoardCommandHandler(logger.Object, unitOfWork.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenBoardDeleted()
        {
            var command = new DeleteBoardCommand
            {
                Id = Guid.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    OwnerId = Guid.Empty,
                    Name = string.Empty
                });

            boardRepository
                .Setup(s => s.Delete(It.IsAny<Board>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("f03b80d1-da7a-4054-ae5c-261310dec753");
            var command = new DeleteBoardCommand
            {
                Id = boardId
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
        }
    }
}
