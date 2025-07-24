using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.BoardAccesses
{
    public class RequestBoardAccessCommandHandlerShould
    {
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IOutboxService> outboxService;
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

            mapper = new Mock<IMapper>();

            outboxService = new Mock<IOutboxService>();
            outboxService
                .Setup(s => s.CreateNewOutboxEvent(It.IsAny<NewBoardAccessRequestEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Parse("fa7a2f2d-5997-4f1d-9622-b197a870982d"));

            logger = new Mock<ILogger<RequestBoardAccessCommandHandler>>();

            sut = new RequestBoardAccessCommandHandler(unitOfWork.Object, mapper.Object, outboxService.Object, logger.Object);
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
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = Guid.Parse("5d0aa8f4-3a54-4037-be6b-940a523c834d"),
                    OwnerId = Guid.Parse("f47a5973-aa9a-4365-bbe7-3d55b5de8f13"),
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            accessRepository.Setup(s => s.Add(It.IsAny<BoardAccessRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapper.Setup(s => s.Map<BoardAccessRequest>(command))
                .Returns(new BoardAccessRequest
                {
                    Id = requestId,
                    BoardId = boardId,
                    AccountId = accountId,
                    AccountName = "Test account name",
                    AccountEmail = "Test account email",
                    Status = 1
                });

            var actual = await sut.Handle(command, CancellationToken.None);
            actual.Should().Be(requestId);
        }

        [Fact]
        public async Task ThrowNotFoundException_WhenBoardNotDoesntExisst()
        {
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = Guid.Empty,
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ThrowForbiddenException_WhenBoardDoesntPublic()
        {
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = Guid.Empty,
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = Guid.Empty,
                    OwnerId = Guid.Empty,
                    Name = "Test board name",
                    Public = false,
                    BoardMembers = []
                });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task ThrowAlreadyExistException_WhenMemberExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = Guid.Empty,
                    OwnerId = Guid.Empty,
                    Name = "Test board name",
                    Public = true,
                    BoardMembers =
                    [
                        new BoardMember
                        {
                            AccountId = accountId,
                            Nickname = "Test nickname"
                        }
                    ]
                });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<AlreadyExistException>();
        }

        [Fact]
        public async Task ThrowAlreadyExistException_WhenAccessRequestExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = Guid.Empty,
                    OwnerId = Guid.Empty,
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardAccessRequest 
                { 
                    BoardId = Guid.Empty,
                    AccountId = accountId,
                    AccountName = "Test account name",
                    AccountEmail = "Test email",
                    Status = 0
                });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<AlreadyExistException>();
        }

        [Fact]
        public async Task ThrowAlreadyExistException_WhenInviteRequestExist()
        {
            var accountId = Guid.Parse("73815cf6-c89d-49d9-b1ac-99ea6f965d10");
            var command = new RequestBoardAccessCommand
            {
                BoardId = Guid.Empty,
                AccountId = accountId,
                AccountName = "Test account name",
                AccountEmail = "Test account email"
            };

            boardRepository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Board
                {
                    Id = Guid.Empty,
                    OwnerId = Guid.Empty,
                    Name = "Test board name",
                    Public = true,
                    BoardMembers = []
                });

            accessRepository
                .Setup(s => s.GetByBoardIdAndAccountId(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            inviteRepository
                .Setup(s => s.GetByBoardIdAndToAccountIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BoardInviteRequest 
                { 
                    BoardId = Guid.Empty,
                    FromAccountId = Guid.Empty,
                    FromAccountName = "Test account name",
                    ToAccountId = Guid.Empty,
                    ToAccountName = "Test account name",
                    ToAccountEmail = "Test account email",
                    Status = 0
                });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<AlreadyExistException>();
        }
    }
}
