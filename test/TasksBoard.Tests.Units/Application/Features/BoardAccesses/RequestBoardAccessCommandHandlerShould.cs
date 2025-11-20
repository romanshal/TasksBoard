using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Constants;
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
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.BoardAccesses
{
    public class RequestBoardAccessCommandHandlerShould
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IOutboxEventFactory> _eventFactory;
        private readonly Mock<ILogger<RequestBoardAccessCommandHandler>> _logger;
        private readonly Mock<IBoardAccessRequestRepository> _accessRepository;
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IBoardInviteRequestRepository> _inviteRepository;
        private readonly Mock<IOutboxEventRepository> _outboxRepository;
        private readonly RequestBoardAccessCommandHandler _sut;

        public RequestBoardAccessCommandHandlerShould()
        {
            _outboxRepository = new Mock<IOutboxEventRepository>();

            _accessRepository = new Mock<IBoardAccessRequestRepository>();
            _boardRepository = new Mock<IBoardRepository>();
            _inviteRepository = new Mock<IBoardInviteRequestRepository>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(_accessRepository.Object);
            _unitOfWork.Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork.Setup(s => s.GetBoardInviteRequestRepository())
                .Returns(_inviteRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId, IOutboxEventRepository>())
                .Returns(_outboxRepository.Object);
            _unitOfWork.Setup(u => u.TransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(),
                It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            _mapper = new Mock<IMapper>();

            _eventFactory = new Mock<IOutboxEventFactory>();
            _eventFactory
                .Setup(s => s.Create(It.IsAny<NewBoardAccessRequestEvent>()))
                .Returns(new OutboxEvent
                {
                    EventType = nameof(NewBoardAccessRequestEvent),
                    Payload = string.Empty,
                    Status = OutboxEventStatuses.Created
                });

            _logger = new Mock<ILogger<RequestBoardAccessCommandHandler>>();

            _sut = new RequestBoardAccessCommandHandler(_unitOfWork.Object, _mapper.Object, _eventFactory.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnRequestBoardAccessGuid_WhenReguestCreated()
        {
            var requestId = Guid.Parse("762f6d5c-1b13-48c6-8d54-c260b3c422de");
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of(accountId),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            _accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            _inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            _accessRepository.Setup(s => s.Add(It.IsAny<BoardAccessRequest>()));

            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId>())
                .Returns(_outboxRepository.Object);

            _mapper
                .Setup(s => s.Map<BoardAccessRequest>(command))
                .Returns(new BoardAccessRequest
                {
                    Id = BoardAccessId.Of(requestId),
                    BoardId = BoardId.Of(boardId),
                    AccountId = AccountId.Of(accountId),
                    Status = 1
                });

            _unitOfWork
                .Setup(s => s.GetRepository<OutboxEvent, OutboxId>())
                .Returns(_outboxRepository.Object);

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(requestId);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardNotDoesntExisst()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }

        [Fact]
        public async Task ReturnForbiddenResult_WhenBoardDoesntPublic()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of(accountId),
                    Name = "Test board name",
                    Public = false,
                    BoardMembers = []
                });

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.Private("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenMemberExist()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var board = new Board
                    {
                        Id = BoardId.Of(boardId),
                        OwnerId = AccountId.Of(accountId),
                        Name = "Test board name",
                        Public = true
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

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardMemberErrors.AlreadyExist("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenAccessRequestExist()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of(accountId),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            _accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardAccessRequest
                {
                    BoardId = BoardId.Of(boardId),
                    AccountId = AccountId.Of(accountId),
                    Status = 0
                });

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardAccessErrors.AlreadyExist("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenInviteRequestExist()
        {
            var boardId = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d");
            var accountId = Guid.Parse("c68505b8-9c99-4fb1-8bc7-923fbdb4e284");
            var command = new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = accountId,
            };

            _boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.Of(accountId),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            _accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            _inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardInviteRequest
                {
                    BoardId = BoardId.Of(boardId),
                    FromAccountId = AccountId.Of(accountId),
                    ToAccountId = AccountId.Of(accountId),
                    Status = 0
                });

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardInviteErrors.AlreadyExist("Test board name"));
        }
    }
}
