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
        private readonly Mock<IBoardRepository> _boardRepository;
        private readonly Mock<IBoardMemberRepository> _boardMemberRepository;
        private readonly Mock<IRepository<BoardPermission, BoardPermissionId>> _boardPermissionRepository;
        private readonly Mock<IRepository<BoardMemberPermission, MemberPermissionId>> _boardMemberPermissionRepository;
        private readonly Mock<ILogger<CreateBoardCommandHandler>> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CreateBoardCommandHandler _sut;

        public CreateBoardCommandHandlerShould()
        {
            _boardRepository = new Mock<IBoardRepository>();
            _boardMemberRepository = new Mock<IBoardMemberRepository>();
            _boardPermissionRepository = new Mock<IRepository<BoardPermission, BoardPermissionId>>();
            _boardMemberPermissionRepository = new Mock<IRepository<BoardMemberPermission, MemberPermissionId>>();

            _logger = new Mock<ILogger<CreateBoardCommandHandler>>();

            _mapper = new Mock<IMapper>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardRepository())
                .Returns(_boardRepository.Object);
            _unitOfWork
                .Setup(s => s.GetBoardMemberRepository())
                .Returns(_boardMemberRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<BoardPermission, BoardPermissionId>())
                .Returns(_boardPermissionRepository.Object);
            _unitOfWork
                .Setup(s => s.GetRepository<BoardMemberPermission, MemberPermissionId>())
                .Returns(_boardMemberPermissionRepository.Object);

            _sut = new CreateBoardCommandHandler(_logger.Object, _unitOfWork.Object, _mapper.Object);
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

            _mapper
                .Setup(s => s.Map<Board>(command))
                .Returns(new Board
                {
                    Id = BoardId.Of(boardId),
                    OwnerId = AccountId.New(),
                    Name = string.Empty
                });

            _boardRepository
                .Setup(s => s.Add(It.IsAny<Board>()));

            _boardMemberRepository
                .Setup(s => s.Add(It.IsAny<BoardMember>()));

            _boardPermissionRepository
                .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _boardMemberPermissionRepository
                .Setup(s => s.Add(It.IsAny<BoardMemberPermission>()));

            _unitOfWork
                .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeEmpty().And.Be(boardId);
        }
    }
}
