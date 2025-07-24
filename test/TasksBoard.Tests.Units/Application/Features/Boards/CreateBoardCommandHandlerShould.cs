using AutoMapper;
using Common.Blocks.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Interfaces.Repositories;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Tests.Units.Application.Features.Boards
{
    public class CreateBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardMemberRepository> boardMemberRepository;
        private readonly Mock<IRepository<BoardPermission>> boardPermissionRepository;
        private readonly Mock<IRepository<BoardMemberPermission>> boardMemberPermissionRepository;
        private readonly Mock<ILogger<CreateBoardCommandHandler>> logger;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly CreateBoardCommandHandler sut;

        public CreateBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();
            boardMemberRepository = new Mock<IBoardMemberRepository>();
            boardPermissionRepository = new Mock<IRepository<BoardPermission>>();
            boardMemberPermissionRepository = new Mock<IRepository<BoardMemberPermission>>();

            logger = new Mock<ILogger<CreateBoardCommandHandler>>();

            mapper = new Mock<IMapper>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetRepository<Board>())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardMember>())
                .Returns(boardMemberRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardPermission>())
                .Returns(boardPermissionRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardMemberPermission>())
                .Returns(boardMemberPermissionRepository.Object);

            sut = new CreateBoardCommandHandler(logger.Object, unitOfWork.Object, mapper.Object);
        }

        [Fact]
        public async Task ReturnCreatedBoardId_WhenBoardCreated()
        {
            var boardId = Guid.Parse("cda27b4a-0865-475b-9562-07edb3a73360");
            var command = new CreateBoardCommand 
            {
                OwnerId = Guid.Empty,
                Name = string.Empty,
                OwnerNickname = string.Empty
            };

            mapper
                .Setup(s => s.Map<Board>(command))
                .Returns(new Board 
                {
                    Id = boardId,
                    OwnerId = Guid.Empty,
                    Name = string.Empty
                });

            boardRepository
                .Setup(s => s.Add(It.IsAny<Board>()));

            boardMemberRepository
                .Setup(s => s.Add(It.IsAny<BoardMember>()));

            boardPermissionRepository
                .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            boardMemberPermissionRepository
                .Setup(s => s.Add(It.IsAny<BoardMemberPermission>()));

            unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await sut.Handle(command, CancellationToken.None);
            actual.Should().Be(boardId);
        }
    }
}
