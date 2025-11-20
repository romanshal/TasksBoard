using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Abstraction.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoards
{
    public class UpdateBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IOutboxEventRepository> _outboxRepository;
        private readonly Mock<ILogger<UpdateBoardCommandHandler>> _logger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UpdateBoardCommandHandler _sut;

        public UpdateBoardCommandHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();
            _outboxRepository = new Mock<IOutboxEventRepository>();

            _logger = new Mock<ILogger<UpdateBoardCommandHandler>>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId, IOutboxEventRepository>())
                .Returns(_outboxRepository.Object);
            _unitOfWork.Setup(u => u.TransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(),
                It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            _sut = new UpdateBoardCommandHandler(_logger.Object, _unitOfWork.Object);
        }

        [Fact]
        public async Task ReturnUpdatedBoardId_WhenBoardUpdated()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new UpdateBoardCommand
            {
                BoardId = boardId,
                Name = string.Empty,
                Tags = []
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of(accountId),
                    Name = string.Empty,
                    BoardTags = []
                });

            _boardRepository.Setup(s => s.Update(It.IsAny<Board>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(boardId);

            _boardRepository.Verify(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _boardRepository.Verify(s => s.Update(It.IsAny<Board>()), Times.Once);
            _boardRepository.VerifyNoOtherCalls();

            _unitOfWork.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _boardRepository.VerifyNoOtherCalls();
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

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);

            _boardRepository.Verify(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _boardRepository.VerifyNoOtherCalls();
        }
    }
}
