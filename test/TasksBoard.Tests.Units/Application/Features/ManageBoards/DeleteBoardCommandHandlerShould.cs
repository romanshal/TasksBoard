using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoards
{
    public class DeleteBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<DeleteBoardCommandHandler>> logger;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IOutboxEventFactory> eventFactory;
        private readonly DeleteBoardCommandHandler sut;

        public DeleteBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<DeleteBoardCommandHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.TransactionAsync(It.IsAny<Func<CancellationToken, Task<Result>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result>> func, CancellationToken ct) => func(ct));

            eventFactory = new Mock<IOutboxEventFactory>();
            eventFactory
                .Setup(s => s.Create(It.IsAny<BaseEvent>()));

            sut = new DeleteBoardCommandHandler(logger.Object, unitOfWork.Object, eventFactory.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenBoardDeleted()
        {
            var boardId = Guid.Parse("c41f3111-74a0-4c03-8bf5-a2055aa37ece");
            var accountId = Guid.Parse("a6be5312-d3d6-4102-8389-19a31f3f1c41");
            var command = new DeleteBoardCommand
            {
                Id = boardId,
                AccountId = accountId
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var board = new Board
                    {
                        Id = BoardId.Of(command.Id),
                        OwnerId = AccountId.Of(command.AccountId),
                        Name = string.Empty,
                    };

                    board.BoardMembers =
                    [
                        new BoardMember
                        {
                            BoardId = board.Id,
                            AccountId = AccountId.Of(accountId)
                        }
                    ];

                    return board;
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
                Id = boardId,
                AccountId = Guid.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
