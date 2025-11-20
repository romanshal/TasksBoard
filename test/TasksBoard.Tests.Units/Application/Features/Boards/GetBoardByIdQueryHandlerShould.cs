using AutoMapper;
using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<ILogger<GetBoardByIdQueryHandler>> _logger;
        private readonly Mock<IUserProfileHandler> _userProfile;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly GetBoardByIdQueryHandler _sut;

        public GetBoardByIdQueryHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();

            _logger = new Mock<ILogger<GetBoardByIdQueryHandler>>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);

            _userProfile = new Mock<IUserProfileHandler>();

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>(), new NullLoggerFactory()));

            _sut = new GetBoardByIdQueryHandler(_logger.Object, _unitOfWork.Object, _mapper, _userProfile.Object);
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
                OwnerId = AccountId.New(),
                Name = string.Empty
            };

            _boardRepository
                .Setup(s => s.GetAsync(board.Id, true, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(board);

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardProfile>(), new NullLoggerFactory()));
            var boardDto = mapper.Map<BoardDto>(board);

            var actual = await _sut.Handle(query, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull().And.BeEquivalentTo(boardDto);

            _boardRepository.Verify(s => s.GetAsync(It.IsAny<BoardId>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _boardRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenBoardDoesntExist()
        {
            var boardId = Guid.Parse("ffe23860-034b-424a-a4c5-bea28307ab0b");
            var query = new GetBoardByIdQuery
            {
                Id = boardId
            };

            _boardRepository
                .Setup(s => s.GetAsync(BoardId.Of(boardId), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            var actual = await _sut.Handle(query, CancellationToken.None);

            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.BeEquivalentTo(BoardErrors.NotFound);
        }
    }
}
