using AutoMapper;
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
using Moq.Language.Flow;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IBoardNoticeRepository> _boardNoticeRepository;
        private readonly Mock<IOutboxEventRepository> _outboxEventRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly ISetup<IMapper, BoardNotice> _mapperSetup;
        private readonly Mock<IOutboxEventFactory> _eventFactory;
        private readonly Mock<ILogger<CreateBoardNoticeCommandHandler>> _logger;
        private readonly CreateBoardNoticeCommandHandler _sut;

        public CreateBoardNoticeCommandHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();
            _boardNoticeRepository = new Mock<IBoardNoticeRepository>();
            _outboxEventRepository = new Mock<IOutboxEventRepository>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork
                .Setup(s => s.GetBoardNoticeRepository())
                .Returns(_boardNoticeRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId, IOutboxEventRepository>())
                .Returns(_outboxEventRepository.Object);

            _unitOfWork
                .Setup(u => u.TransactionAsync(It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            _mapper = new Mock<IMapper>();
            _mapperSetup = _mapper.Setup(s => s.Map<BoardNotice>(It.IsAny<CreateBoardNoticeCommand>()));

            _eventFactory = new Mock<IOutboxEventFactory>();
            _eventFactory
                .Setup(s => s.Create(It.IsAny<NewNoticeEvent>()));

            _logger = new Mock<ILogger<CreateBoardNoticeCommandHandler>>();

            _sut = new CreateBoardNoticeCommandHandler(_unitOfWork.Object, _mapper.Object, _eventFactory.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnBoardNoticeId_WhenBoardNoticeCreated()
        {
            var boardId = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019");
            var command = new CreateBoardNoticeCommand
            {
                AuthorId = Guid.Empty,
                AuthorName = string.Empty,
                BoardId = boardId,
                Definition = string.Empty,
                BackgroundColor = string.Empty,
                Rotation = string.Empty
            };
            var noticeId = Guid.Parse("5b887771-030f-469d-9986-eeb6218ec0f8");

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of("332bb7fa-cd78-4fe9-a4d7-475ae374a019"),
                    Name = string.Empty,
                    BoardMembers = []
                });

            _mapperSetup.Returns(new BoardNotice
            {
                Id = BoardNoticeId.Of(noticeId),
                AuthorId = AccountId.New(),
                BoardId = BoardId.New(),
                Definition = string.Empty,
                BackgroundColor = string.Empty,
                Rotation = string.Empty
            });

            _boardNoticeRepository
                .Setup(s => s.Add(It.IsAny<BoardNotice>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

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

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
