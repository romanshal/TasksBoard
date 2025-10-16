using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoardNotices
{
    public class CreateBoardNoticeCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardNoticeRepository> boardNoticeRepository;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IMapper> mapper;
        private readonly ISetup<IMapper, BoardNotice> mapperSetup;
        private readonly Mock<IOutboxEventFactory> eventFactory;
        private readonly Mock<ILogger<CreateBoardNoticeCommandHandler>> logger;
        private readonly CreateBoardNoticeCommandHandler sut;

        public CreateBoardNoticeCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();
            boardNoticeRepository = new Mock<IBoardNoticeRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board, BoardId>())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardNotice, BoardNoticeId>())
                .Returns(boardNoticeRepository.Object);
            unitOfWork
                .Setup(u => u.TransactionAsync(It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            mapper = new Mock<IMapper>();
            mapperSetup = mapper.Setup(s => s.Map<BoardNotice>(It.IsAny<CreateBoardNoticeCommand>()));

            eventFactory = new Mock<IOutboxEventFactory>();
            eventFactory
                .Setup(s => s.Create(It.IsAny<NewNoticeEvent>()));

            logger = new Mock<ILogger<CreateBoardNoticeCommandHandler>>();

            sut = new CreateBoardNoticeCommandHandler(unitOfWork.Object, mapper.Object, eventFactory.Object, logger.Object);
        }

        [Fact]
        public async Task ReturnBoardNoticeId_WhenBoardNoticeCreated()
        {
            var command = new CreateBoardNoticeCommand
            {
                AuthorId = Guid.Empty,
                AuthorName = string.Empty,
                BoardId = Guid.Empty,
                Definition = string.Empty,
                BackgroundColor = string.Empty,
                Rotation = string.Empty
            };
            var noticeId = Guid.Parse("5b887771-030f-469d-9986-eeb6218ec0f8");

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    OwnerId = AccountId.New(),
                    Name = string.Empty,
                    BoardMembers = []
                });

            mapperSetup.Returns(new BoardNotice
            {
                Id = BoardNoticeId.Of(noticeId),
                AuthorId = AccountId.New(),
                BoardId = BoardId.New(),
                Definition = string.Empty,
                BackgroundColor = string.Empty,
                Rotation = string.Empty
            });

            boardNoticeRepository
                .Setup(s => s.Add(It.IsAny<BoardNotice>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(noticeId);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("7bd28725-ed98-4c8f-b459-f3947a157ca7");
            var command = new CreateBoardNoticeCommand
            {
                AuthorId = Guid.Empty,
                AuthorName = string.Empty,
                BoardId = boardId,
                Definition = string.Empty,
                BackgroundColor = string.Empty,
                Rotation = string.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
