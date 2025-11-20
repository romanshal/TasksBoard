using AutoMapper;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.Boards
{
    public class CreateBoardCommandHandlerShould
    {
        private readonly Mock<IBoardRepository> boardRepository;
        private readonly Mock<IBoardMemberRepository> boardMemberRepository;
        private readonly Mock<IRepository<BoardPermission, BoardPermissionId>> boardPermissionRepository;
        private readonly Mock<IRepository<BoardMemberPermission, MemberPermissionId>> boardMemberPermissionRepository;
        private readonly Mock<ILogger<CreateBoardCommandHandler>> logger;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly CreateBoardCommandHandler sut;

        public CreateBoardCommandHandlerShould()
        {
            boardRepository = new Mock<IBoardRepository>();
            boardMemberRepository = new Mock<IBoardMemberRepository>();
            boardPermissionRepository = new Mock<IRepository<BoardPermission, BoardPermissionId>>();
            boardMemberPermissionRepository = new Mock<IRepository<BoardMemberPermission, MemberPermissionId>>();

            logger = new Mock<ILogger<CreateBoardCommandHandler>>();

            mapper = new Mock<IMapper>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(boardRepository.Object);
            unitOfWork
                .Setup(s => s.GetBoardMemberRepository())
                .Returns(boardMemberRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardPermission, BoardPermissionId>())
                .Returns(boardPermissionRepository.Object);
            unitOfWork
                .Setup(s => s.GetRepository<BoardMemberPermission, MemberPermissionId>())
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
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.New(),
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

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(boardId);
        }
    }
}
