using Common.Blocks.Models.DomainResults;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

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
            unitOfWork.Setup(u => u.TransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

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
                .Setup(s => s.Update(It.IsAny<BoardAccessRequest>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(requestId);

            repository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            repository.Verify(s => s.Update(It.IsAny<BoardAccessRequest>()), Times.Once);
            repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenRequestDoesntExist()
        {
            var requestId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037");
            var command = new CancelBoardAccessCommand
            {
                RequestId = requestId
            };

            repositoryGetSetup
                     .ReturnsAsync((BoardAccessRequest)null!);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsFailure.Should().BeTrue();
            actual.Error.Should().Be(BoardAccessErrors.NotFound);

            repository.Verify(r => r.GetAsync(requestId, It.IsAny<CancellationToken>()), Times.Once);
            repository.Verify(r => r.Update(It.IsAny<BoardAccessRequest>()), Times.Never);
            unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
