﻿using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoards
{
    public class DeleteBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<DeleteBoardCommandHandler>> logger;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IOutboxService> outboxService;
        private readonly DeleteBoardCommandHandler sut;

        public DeleteBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<DeleteBoardCommandHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.TransactionAsync(It.IsAny<Func<CancellationToken, Task<Result>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result>> func, CancellationToken ct) => func(ct));

            outboxService = new Mock<IOutboxService>();
            outboxService
                .Setup(s => s.CreateNewOutboxEvent(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            sut = new DeleteBoardCommandHandler(logger.Object, unitOfWork.Object, outboxService.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenBoardDeleted()
        {
            var command = new DeleteBoardCommand
            {
                Id = Guid.Empty,
                AccountId = Guid.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    OwnerId = Guid.Empty,
                    Name = string.Empty,
                    BoardMembers =
                    [
                        new BoardMember
                        {
                            AccountId = Guid.Empty,
                            Nickname = "Test"
                        }
                    ]
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
