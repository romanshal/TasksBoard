using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoardNotices
{
    public class DeleteBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardNoticeRepository> boardNoticeRepository;
        private readonly Mock<ILogger<DeleteBoardNoticeCommandHandler>> logger;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly DeleteBoardNoticeCommandHandler sut;

        public DeleteBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();
            boardNoticeRepository = new Mock<IBoardNoticeRepository>();

            logger = new Mock<ILogger<DeleteBoardNoticeCommandHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardNotice>())
                .Returns(boardNoticeRepository.Object);

            sut = new DeleteBoardNoticeCommandHandler(logger.Object, unitOfWork.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenBoardNoticeDeleted()
        {
            var command = new DeleteBoardNoticeCommand
            {
                BoardId = Guid.Empty,
                NoticeId = Guid.Empty
            };

            boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            boardNoticeRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardNotice
                {
                    AuthorId = Guid.Empty,
                    AuthorName = string.Empty,
                    BoardId = Guid.Empty,
                    Definition = string.Empty,
                    BackgroundColor = string.Empty,
                    Rotation = string.Empty
                });

            boardNoticeRepository
                .Setup(s => s.Delete(It.IsAny<BoardNotice>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

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

            boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: false);

            //await sut
            //    .Invoking(s => s.Handle(command, CancellationToken.None))
            //    .Should()
            //    .ThrowAsync<NotFoundException>()
            //    .WithMessage($"Board with id '{boardId}' not found.");

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardNoticeDoesntExist()
        {
            var noticeId = Guid.Parse("6333298d-a54b-47fb-aaab-20a205fdca83");
            var command = new DeleteBoardNoticeCommand
            {
                BoardId = Guid.Empty,
                NoticeId = noticeId
            };

            boardRepository
                .Setup(s => s.ExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            boardNoticeRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            //await sut
            //    .Invoking(s => s.Handle(command, CancellationToken.None))
            //    .Should()
            //    .ThrowAsync<NotFoundException>()
            //    .WithMessage($"Board notice with id '{noticeId}' not found.");

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardNoticeErrors.NotFound);
        }
    }
}
