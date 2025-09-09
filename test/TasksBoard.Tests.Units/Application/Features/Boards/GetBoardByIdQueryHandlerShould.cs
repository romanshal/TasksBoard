using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Mappings;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Tests.Units.Application.Features.Boards
{
    public class GetBoardByIdQueryHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<ILogger<GetBoardByIdQueryHandler>> logger;
        private readonly IMapper mapper;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly GetBoardByIdQueryHandler sut;

        public GetBoardByIdQueryHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();

            logger = new Mock<ILogger<GetBoardByIdQueryHandler>>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);

            //mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>()));

            //sut = new GetBoardByIdQueryHandler(logger.Object, unitOfWork.Object, mapper);
        }

        [Fact]
        public async Task ReturnBoard_WhenBoardExist()
        {
            //var boardId = Guid.Parse("ffe23860-034b-424a-a4c5-bea28307ab0b");
            //var query = new GetBoardByIdQuery
            //{
            //    Id = boardId
            //};

            //var board = new Board
            //{
            //    Id = boardId,
            //    OwnerId = Guid.Empty,
            //    Name = string.Empty
            //};

            //boardRepository
            //    .Setup(s => s.GetAsync(boardId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(board);

            //var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>()));
            //var boardDto = mapper.Map<BoardDto>(board);

            //var actual = await sut.Handle(query, CancellationToken.None);

            //actual.IsSuccess.Should().BeTrue();
            //actual.Value.Should().NotBeNull().And.BeEquivalentTo(boardDto);

            //boardRepository.Verify(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            //boardRepository.VerifyNoOtherCalls();
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
                .Setup(s => s.GetAsync(boardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            //await sut
            //    .Invoking(s => s.Handle(query, CancellationToken.None))
            //    .Should()
            //    .ThrowAsync<NotFoundException>()
            //    .WithMessage($"Board with id '{boardId}' was not found.");

            var actual = await sut.Handle(query, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
