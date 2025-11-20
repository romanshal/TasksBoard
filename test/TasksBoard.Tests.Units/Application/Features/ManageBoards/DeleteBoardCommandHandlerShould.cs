using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.Interfaces.Factories;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Abstraction.ValueObjects;
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
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IOutboxEventRepository> _outboxRepository;
        private readonly Mock<ILogger<DeleteBoardCommandHandler>> _logger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IOutboxEventFactory> _eventFactory;
        private readonly DeleteBoardCommandHandler _sut;

        public DeleteBoardCommandHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();
            _outboxRepository = new Mock<IOutboxEventRepository>();

            _logger = new Mock<ILogger<DeleteBoardCommandHandler>>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId, IOutboxEventRepository>())
                .Returns(_outboxRepository.Object);
            _unitOfWork
                .Setup(s => s.TransactionAsync(It.IsAny<Func<CancellationToken, Task<Result>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result>> func, CancellationToken ct) => func(ct));

            _eventFactory = new Mock<IOutboxEventFactory>();
            _eventFactory
                .Setup(s => s.Create(It.IsAny<BaseEvent>()));

            _sut = new DeleteBoardCommandHandler(_logger.Object, _unitOfWork.Object, _eventFactory.Object);
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

            _boardRepository
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

            _boardRepository
                .Setup(s => s.Delete(It.IsAny<Board>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

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

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
