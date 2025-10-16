using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IOutboxEventFactory> eventFactory;
        private readonly Mock<ILogger<RequestBoardAccessCommandHandler>> logger;
        private readonly Mock<IBoardAccessRequestRepository> accessRepository;
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardInviteRequestRepository> inviteRepository;
        private readonly RequestBoardAccessCommandHandler sut;

        public RequestBoardAccessCommandHandlerShould()
        {
            accessRepository = new Mock<IBoardAccessRequestRepository>();
            boardRepository = new Mock<IBoardRepository>();
            inviteRepository = new Mock<IBoardInviteRequestRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(accessRepository.Object);
            unitOfWork.Setup(s => s.GetBoardRepository())
                .Returns(boardRepository.Object);
            unitOfWork.Setup(s => s.GetBoardInviteRequestRepository())
                .Returns(inviteRepository.Object);
            unitOfWork.Setup(u => u.TransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(),
                It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            mapper = new Mock<IMapper>();

            eventFactory = new Mock<IOutboxEventFactory>();
            eventFactory
                .Setup(s => s.Create(It.IsAny<NewBoardAccessRequestEvent>()));

            logger = new Mock<ILogger<RequestBoardAccessCommandHandler>>();

            sut = new RequestBoardAccessCommandHandler(unitOfWork.Object, mapper.Object, eventFactory.Object, logger.Object);
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

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.Of("5d0aa8f4-3a54-4037-be6b-940a523c834d"),
                    OwnerId = AccountId.Of("f47a5973-aa9a-4365-bbe7-3d55b5de8f13"),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            accessRepository.Setup(s => s.Add(It.IsAny<BoardAccessRequest>()));

            mapper.Setup(s => s.Map<BoardAccessRequest>(command))
                .Returns(new BoardAccessRequest
                {
                    Id = BoardAccessId.Of(requestId),
                    BoardId = BoardId.Of(boardId),
                    AccountId = AccountId.Of(accountId),
                    Status = 1
                });

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(requestId);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardNotDoesntExisst()
        {
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = Guid.Empty,
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }

        [Fact]
        public async Task ReturnForbiddenResult_WhenBoardDoesntPublic()
        {
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = Guid.Empty,
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.New(),
                    OwnerId = AccountId.New(),
                    Name = "Test board name",
                    Public = false,
                    BoardMembers = []
                });

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.Private("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenMemberExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var board = new Board
                    {
                        Id = BoardId.New(),
                        OwnerId = AccountId.New(),
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

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardMemberErrors.AlreadyExist("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenAccessRequestExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.New(),
                    OwnerId = AccountId.New(),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardAccessRequest
                {
                    BoardId = BoardId.New(),
                    AccountId = AccountId.Of(accountId),
                    Status = 0
                });

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardAccessErrors.AlreadyExist("Test board name"));
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenInviteRequestExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = BoardId.New(),
                    OwnerId = AccountId.New(),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<BoardId>(), It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardInviteRequest
                {
                    BoardId = BoardId.New(),
                    FromAccountId = AccountId.New(),
                    ToAccountId = AccountId.New(),
                    Status = 0
                });

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardInviteErrors.AlreadyExist("Test board name"));
        }
    }
}
