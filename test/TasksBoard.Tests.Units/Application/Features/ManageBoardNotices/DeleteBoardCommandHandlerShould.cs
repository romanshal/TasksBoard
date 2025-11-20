using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardNotice;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoardNotices
{
    public class DeleteBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IBoardNoticeRepository> _boardNoticeRepository;
        private readonly Mock<ILogger<DeleteBoardNoticeCommandHandler>> _logger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly DeleteBoardNoticeCommandHandler _sut;

        public DeleteBoardCommandHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();
            _boardNoticeRepository = new Mock<IBoardNoticeRepository>();

            _logger = new Mock<ILogger<DeleteBoardNoticeCommandHandler>>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork
                .Setup(s => s.GetBoardNoticeRepository())
                .Returns(_boardNoticeRepository.Object);

            _sut = new DeleteBoardNoticeCommandHandler(_logger.Object, _unitOfWork.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenBoardNoticeDeleted()
        {
            var boardId = Guid.Parse("a6be5312-d3d6-4102-8389-19a31f3f1c41");
            var noticeId = Guid.Parse("c41f3111-74a0-4c03-8bf5-a2055aa37ece");
            var command = new DeleteBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = noticeId
            };

            _boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            _boardNoticeRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardNoticeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardNotice
                {
                    AuthorId = AccountId.New(),
                    BoardId = BoardId.Of(boardId),
                    Definition = string.Empty,
                    BackgroundColor = string.Empty,
                    Rotation = string.Empty
                });

            _boardNoticeRepository
                .Setup(s => s.Delete(It.IsAny<BoardNotice>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("6333298d-a54b-47fb-aaab-20a205fdca83");
            var command = new DeleteBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = Guid.Empty
            };

            _boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: false);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardNoticeDoesntExist()
        {
            var boardId = Guid.Parse("6333298d-a54b-47fb-aaab-20a205fdca83");
            var noticeId = Guid.Parse("6333298d-a54b-47fb-aaab-20a205fdca83");
            var command = new DeleteBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = noticeId
            };

            _boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            _boardNoticeRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardNoticeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardNoticeErrors.NotFound);
        }
    }
}
