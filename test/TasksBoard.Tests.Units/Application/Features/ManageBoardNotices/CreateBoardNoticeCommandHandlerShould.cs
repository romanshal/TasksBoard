using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Application.Mappings;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.ManageBoardNotices
{
    public class CreateBoardNoticeCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardNoticeRepository> boardNoticeRepository;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IMapper> mapper;
        private readonly ISetup<IMapper, BoardNotice> mapperSetup;
        private readonly Mock<IOutboxService> outboxService;
        private readonly Mock<ILogger<CreateBoardNoticeCommandHandler>> logger;
        private readonly CreateBoardNoticeCommandHandler sut;

        public CreateBoardNoticeCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();
            boardNoticeRepository = new Mock<IBoardNoticeRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardNotice>())
                .Returns(boardNoticeRepository.Object);

            mapper = new Mock<IMapper>();
            mapperSetup = mapper.Setup(s => s.Map<BoardNotice>(It.IsAny<CreateBoardNoticeCommand>()));

            outboxService = new Mock<IOutboxService>();
            outboxService
                .Setup(s => s.CreateNewOutboxEvent(It.IsAny<NewNoticeEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            logger = new Mock<ILogger<CreateBoardNoticeCommandHandler>>();

            sut = new CreateBoardNoticeCommandHandler(unitOfWork.Object, mapper.Object, outboxService.Object, logger.Object);
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
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    OwnerId = Guid.Empty,
                    Name = string.Empty,
                    BoardMembers = []
                });

            mapperSetup.Returns(new BoardNotice 
            {
                Id = noticeId,
                AuthorId = Guid.Empty,
                AuthorName = string.Empty,
                BoardId = Guid.Empty,
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
            actual.Should().Be(noticeId);
        }

        [Fact]
        public async Task ThrowNotFoundException_WhenBoardDoesntExist()
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
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var t = await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Board with id '{boardId}' not found.");
        }
    }
}
