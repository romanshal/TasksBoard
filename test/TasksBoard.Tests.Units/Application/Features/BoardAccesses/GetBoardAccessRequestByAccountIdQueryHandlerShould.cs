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
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mapper mapper;
        private readonly Mock<IUserProfileHandler> userProfile;
        private readonly Mock<IBoardAccessRequestRepository> accessRepository;
        private readonly Mock<ILogger<GetBoardAccessRequestByAccountIdQueryHandler>> logger;
        private readonly GetBoardAccessRequestByAccountIdQueryHandler sut;

        public GetBoardAccessRequestByAccountIdQueryHandlerShould()
        {
            accessRepository = new Mock<IBoardAccessRequestRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .Setup(s => s.GetBoardAccessRequestRepository())
                .Returns(accessRepository.Object);

            logger = new Mock<ILogger<GetBoardAccessRequestByAccountIdQueryHandler>>();

            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<BoardAccessRequestProfile>(), new NullLoggerFactory()));

            userProfile = new Mock<IUserProfileHandler>();
            userProfile
                .Setup(h => h.Handle(
                    It.IsAny<IEnumerable<BaseDto>>(),
                    It.IsAny<Func<BaseDto, Guid>>(),
                    It.IsAny<Action<BaseDto, string, string?>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            sut = new GetBoardAccessRequestByAccountIdQueryHandler(unitOfWork.Object, logger.Object, mapper, userProfile.Object);
        }

        [Fact]
        public async Task RetunListOfAccessRequests_WhenAccessRequestsExist()
        {
            var command = new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = Guid.Empty
            };

            var list = new List<BoardAccessRequest>
            {
                new() {
                    BoardId = BoardId.New(),
                    AccountId = AccountId.New(),
                    Status = 0
                }
            };

            accessRepository
                .Setup(s => s.GetByAccountIdAsync(It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var listDto = mapper.Map<IEnumerable<BoardAccessRequestDto>>(list);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNullOrEmpty().And.BeEquivalentTo(listDto);
        }

        [Fact]
        public async Task ReturnEmptyList_WhenAccessRequestsDoesntExist()
        {
            var command = new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = Guid.Empty
            };

            var list = new List<BoardAccessRequest>();

            accessRepository
                .Setup(s => s.GetByAccountIdAsync(It.IsAny<AccountId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var listDto = mapper.Map<IEnumerable<BoardAccessRequestDto>>(list);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().BeEmpty().And.BeEquivalentTo(listDto);
        }
    }
}
