using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Handlers;
using TasksBoard.Application.Mappings;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.Boards
{
    public class GetBoardByIdQueryHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<GetBoardByIdQueryHandler>> logger;
        private readonly Mock<IUserProfileHandler> userProfile;
        private readonly IMapper mapper;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly GetBoardByIdQueryHandler sut;

        public GetBoardByIdQueryHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<GetBoardByIdQueryHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board, BoardId>())
                .Returns(boardRepository.Object);

            userProfile = new Mock<IUserProfileHandler>();

            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>(), new NullLoggerFactory()));

            sut = new GetBoardByIdQueryHandler(logger.Object, unitOfWork.Object, mapper, userProfile.Object);
        }

        [Fact]
        public async Task ReturnBoard_WhenBoardExist()
        {
            var boardId = Guid.Parse("ffe23860-034b-424a-a4c5-bea28307ab0b");
            var query = new GetBoardByIdQuery
            {
                Id = boardId
            };

            var board = new Board
            {
                Id = BoardId.Of(boardId),
                OwnerId = Guid.Empty,
                Name = string.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(board.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(board);

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>(), new NullLoggerFactory()));
            var boardDto = mapper.Map<BoardDto>(board);

            var actual = await sut.Handle(query, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull().And.BeEquivalentTo(boardDto);

            boardRepository.Verify(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<CancellationToken>()), Times.Once);
            boardRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("ffe23860-034b-424a-a4c5-bea28307ab0b");
            var query = new GetBoardByIdQuery
            {
                Id = boardId
            };

            boardRepository
                .Setup(s => s.GetAsync(BoardId.Of(boardId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await sut.Handle(query, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
