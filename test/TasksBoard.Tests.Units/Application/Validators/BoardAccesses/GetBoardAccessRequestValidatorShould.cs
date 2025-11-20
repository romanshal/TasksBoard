using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId;
using TasksBoard.Application.Validators.BoardAccesses;

namespace TasksBoard.Tests.Units.Application.Validators.BoardAccesses
{
    public class GetBoardAccessRequestValidatorShould
    {
        private readonly GetBoardAccessRequestValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1")
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardAccessRequestByAccountIdQuery command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardAccessRequestByAccountIdQuery { AccountId = Guid.NewGuid() };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
        }
    }
}
