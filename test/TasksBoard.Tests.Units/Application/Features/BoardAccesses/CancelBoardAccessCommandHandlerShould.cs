using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.BoardAccesses
{
    public class CancelBoardAccessCommandHandlerShould
    {
        private readonly CancelBoardAccessCommandHandler _sut;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ILogger<CancelBoardAccessCommandHandler>> _logger;
        private readonly Mock<IBoardAccessRequestRepository> _repository;
        private readonly Moq.Language.Flow.ISetup<IBoardAccessRequestRepository, Task<BoardAccessRequest?>> _repositoryGetSetup;

        public CancelBoardAccessCommandHandlerShould()
        {
            _repository = new Mock<IBoardAccessRequestRepository>();
            _repositoryGetSetup = _repository
                .Setup(s => s.GetAsync(It.IsAny<BoardAccessId>(), It.IsAny<CancellationToken>()));

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(_repository.Object);
            _unitOfWork.Setup(u => u.TransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Result<Guid>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<Result<Guid>>> func, CancellationToken ct) => func(ct));

            _logger = new Mock<ILogger<CancelBoardAccessCommandHandler>>();

            _sut = new CancelBoardAccessCommandHandler(_unitOfWork.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnCanceledBoardAccessGuid_WhenRequestExist()
        {
            var requestId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037");
            var command = new CancelBoardAccessCommand
            {
                RequestId = requestId
            };

            _repositoryGetSetup.ReturnsAsync(new BoardAccessRequest
            {
                Id = BoardAccessId.Of(requestId),
                BoardId = BoardId.New(),
                AccountId = AccountId.New(),
                Status = 1
            });

            _repository
                .Setup(s => s.Update(It.IsAny<BoardAccessRequest>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(requestId);

            _repository.Verify(s => s.GetAsync(It.IsAny<BoardAccessId>(), It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(s => s.Update(It.IsAny<BoardAccessRequest>()), Times.Once);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenRequestDoesntExist()
        {
            var requestId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037");
            var command = new CancelBoardAccessCommand
            {
                RequestId = requestId
            };

            _repositoryGetSetup
                     .ReturnsAsync((BoardAccessRequest)null!);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsFailure.Should().BeTrue();
            actual.Error.Should().Be(BoardAccessErrors.NotFound);

            _repository.Verify(r => r.GetAsync(BoardAccessId.Of(requestId), It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<BoardAccessRequest>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
