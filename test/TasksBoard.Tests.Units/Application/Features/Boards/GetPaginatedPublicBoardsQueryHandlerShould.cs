using AutoMapper;
using Common.Blocks.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Application.Mappings;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.Boards
{
    public class GetPaginatedPublicBoardsQueryHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<GetPaginatedPublicBoardsQueryHandler>> logger;
        private readonly IMapper mapper;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly GetPaginatedPublicBoardsQueryHandler sut;

        public GetPaginatedPublicBoardsQueryHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<GetPaginatedPublicBoardsQueryHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);

            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>()));

            sut = new GetPaginatedPublicBoardsQueryHandler(logger.Object, unitOfWork.Object, mapper);
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
                Id = boardId,
                OwnerId = Guid.Empty,
                Name = string.Empty
            };

            boardRepository
                .Setup(s => s.GetAsync(boardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(board);

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>()));
            var boardDto = mapper.Map<BoardDto>(board);

            var actual = await sut.Handle(query, CancellationToken.None);
            actual.Should().BeEquivalentTo(boardDto);

            boardRepository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            boardRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ThrowNotFoundException_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("ffe23860-034b-424a-a4c5-bea28307ab0b");
            var query = new GetBoardByIdQuery
            {
                Id = boardId
            };

            boardRepository
                .Setup(s => s.GetAsync(boardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            await sut
                .Invoking(s => s.Handle(query, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Board with id '{boardId}' was not found.");
        }
    }
}
