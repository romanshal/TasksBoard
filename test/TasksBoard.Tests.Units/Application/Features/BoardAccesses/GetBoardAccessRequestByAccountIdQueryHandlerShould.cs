using AutoMapper;
using Common.Blocks.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId;
using TasksBoard.Application.Handlers;
using TasksBoard.Application.Mappings;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Tests.Units.Application.Features.BoardAccesses
{
    public class GetBoardAccessRequestByAccountIdQueryHandlerShould
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mapper _mapper;
        private readonly Mock<IUserProfileHandler> _userProfile;
        private readonly Mock<IBoardAccessRequestRepository> _accessRepository;
        private readonly Mock<ILogger<GetBoardAccessRequestByAccountIdQueryHandler>> _logger;
        private readonly GetBoardAccessRequestByAccountIdQueryHandler _sut;

        public GetBoardAccessRequestByAccountIdQueryHandlerShould()
        {
            _accessRepository = new Mock<IBoardAccessRequestRepository>();

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork
                .Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(_accessRepository.Object);

            _logger = new Mock<ILogger<GetBoardAccessRequestByAccountIdQueryHandler>>();

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardAccessRequestProfile>(), new NullLoggerFactory()));

            _userProfile = new Mock<IUserProfileHandler>();
            _userProfile
                .Setup(h => h.Handle(
                    It.IsAny<IEnumerable<BaseDto>>(),
                    It.IsAny<Func<BaseDto, Guid>>(),
                    It.IsAny<Action<BaseDto, string, string?>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new GetBoardAccessRequestByAccountIdQueryHandler(_unitOfWork.Object, _logger.Object, _mapper, _userProfile.Object);
        }

        [Fact]
        public async Task RetunListOfAccessRequests_WhenAccessRequestsExist()
        {
            var accountId = Guid.Parse("db85c851-c641-477f-ae33-f38e59d7a74e");
            var boardId = Guid.Parse("d5e625d3-bdba-404a-afe1-4f558b5ec089");
            var command = new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = accountId
            };

            var list = new List<BoardAccessRequest>
            {
                new() {
                    BoardId = BoardId.Of(boardId),
                    AccountId = AccountId.Of(accountId),
                    Status = 0
                }
            };

            _accessRepository
                .Setup(s => s.GetByAccountIdAsync(It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var listDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(list);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNullOrEmpty().And.BeEquivalentTo(listDto);
        }

        [Fact]
        public async Task ReturnEmptyList_WhenAccessRequestsDoesntExist()
        {
            var accountId = Guid.Parse("db85c851-c641-477f-ae33-f38e59d7a74e");
            var command = new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = accountId
            };

            var list = new List<BoardAccessRequest>();

            _accessRepository
                .Setup(s => s.GetByAccountIdAsync(It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var listDto = _mapper.Map<IEnumerable<BoardAccessRequestDto>>(list);

            var actual = await _sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().BeEmpty().And.BeEquivalentTo(listDto);
        }
    }
}
