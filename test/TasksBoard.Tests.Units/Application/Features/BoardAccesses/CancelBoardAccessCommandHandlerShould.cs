using Common.Blocks.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.BoardAccesses
{
    public class CancelBoardAccessCommandHandlerShould
    {
        private readonly CancelBoardAccessCommandHandler sut;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<ILogger<CancelBoardAccessCommandHandler>> logger;
        private readonly Mock<IBoardAccessRequestRepository> repository;
        private readonly Moq.Language.Flow.ISetup<IBoardAccessRequestRepository, Task<BoardAccessRequest?>> repositoryGetSetup;

        public CancelBoardAccessCommandHandlerShould()
        {
            repository = new Mock<IBoardAccessRequestRepository>();
            repositoryGetSetup = repository
                .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(repository.Object);

            logger = new Mock<ILogger<CancelBoardAccessCommandHandler>>();

            sut = new CancelBoardAccessCommandHandler(unitOfWork.Object, logger.Object);
        }

        [Fact]
        public async Task ReturnCanceledBoardAccessGuid_WhenRequestExist()
        {
            var requestId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037");
            var command = new CancelBoardAccessCommand
            {
                RequestId = requestId
            };

            repositoryGetSetup.ReturnsAsync(new BoardAccessRequest
            {
                Id = requestId,
                BoardId = Guid.Empty,
                AccountId = Guid.Empty,
                AccountName = string.Empty,
                AccountEmail = string.Empty,
                Status = 1
            });

            repository
                .Setup(s => s.Update(It.IsAny<BoardAccessRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.Should().Be(requestId);

            repository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            repository.Verify(s => s.Update(It.IsAny<BoardAccessRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundException_WhenRequestDoesntExist()
        {
            var command = new CancelBoardAccessCommand
            {
                RequestId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };
            repositoryGetSetup.ReturnsAsync(value: null);

           var t = await sut
                .Invoking(s => s.Handle(command, It.IsAny<CancellationToken>()))
                .Should()
                .ThrowAsync<NotFoundException>();

            repository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
